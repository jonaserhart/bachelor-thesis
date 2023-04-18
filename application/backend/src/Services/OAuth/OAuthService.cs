using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Web;
using backend.Model;
using backend.Model.Config;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Model.Users;
using backend.Services.API;
using backend.Services.Database;
using backend.Services.Users;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace backend.Services.OAuth;

public class OAuthService : IOAuthService
{
    private readonly ILogger<OAuthService> _logger;
    private readonly DataContext _context;
    private readonly OAuthConfig _oauthConfig;
    private readonly IUserService _userService;
    private readonly IApiClientFactory _apiClientFactory;

    
    private static readonly ConcurrentDictionary<Guid, TokenModel> s_authorizationRequests = new();
    private static readonly HttpClient s_httpClient = new HttpClient();
    
    public OAuthService(DataContext context, ILogger<OAuthService> logger, IOptions<OAuthConfig> oauthConfig, IUserService userService, IApiClientFactory apiClientFactory)
    {
        _context = context;
        _logger = logger;
        _oauthConfig = oauthConfig.Value;
        _userService = userService;
        _apiClientFactory = apiClientFactory;
    }

    /// <summary>
    /// Get the url for a user to authorize this application to access the Azure DevOps API
    /// </summary>
    /// <returns>the url with queryparams</returns>
    public string GetNewAuthorizeUrl()
    {
        var state = Guid.NewGuid();
        s_authorizationRequests[state] = new TokenModel { IsPending = true };

        var uriBuilder = new UriBuilder(_oauthConfig.AuthorizationUri);
        var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? String.Empty);

        queryParams["client_id"] = _oauthConfig.ClientId;
        queryParams["response_type"] = "Assertion";
        queryParams["state"] = state.ToString();
        queryParams["scope"] = _oauthConfig.Scope;
        queryParams["redirect_uri"] = _oauthConfig.RedirectUri;

        uriBuilder.Query = queryParams.ToString();

        return uriBuilder.ToString();
    }

    /// <summary>
    /// Handle callback of OAuth provider
    /// </summary>
    /// <param name="submission"></param>
    /// <returns>A token for the user to authenticate</returns>
    public async Task<TokenModel> HandleOAuthCallback(OAuthCallbackModel submission)
    {
        var code = submission.Code;
        var state = submission.State;
        _logger.LogInformation($"Requested callback for auth request ${state}");
        foreach (var entry in s_authorizationRequests)
        {
            _logger.LogInformation($"{entry.Key}: {entry.Value.IsPending}");
        }

        if (!CallbackValuesAreValid(code, state.ToString(), out var error))
        {
            _logger.LogError($"Error when validating parameters: '{error}'");
            throw new BadRequestException(error);
        }
        
        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _oauthConfig.TokenUri);
        tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var form = new Dictionary<string, string>
        {
            { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
            { "client_assertion", _oauthConfig.ClientSecret },
            { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
            { "assertion", code },
            { "redirect_uri", _oauthConfig.RedirectUri }
        };

        tokenRequest.Content = new FormUrlEncodedContent(form);

        var response = await s_httpClient.SendAsync(tokenRequest);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Token request failed: {response}");
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Content: {content}");
            throw new BadRequestException($"Token request failed: {response.ReasonPhrase} {response.StatusCode}");
        }
        
        var body = await response.Content.ReadAsStringAsync();

        var token = s_authorizationRequests[state];
        JsonConvert.PopulateObject(body, token);

        var apiClient = await _apiClientFactory.GetApiClientAsync(token.AccessToken);
        var user = await apiClient.GetSelfAsync();

        await _userService.CreateOrUpdateUser(user);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = token.RefreshToken,
            IsActive = true
        });
        await _context.SaveChangesAsync();

        return token;
    }

    /// <summary>
    /// Checks if the callback values from the Azure OAuth provider are valid
    /// </summary>
    /// <param name="code"></param>
    /// <param name="state"></param>
    /// <param name="error"></param>
    /// <returns>true, if the paramers are valid, false otherwise</returns>
    private static bool CallbackValuesAreValid(string code, string state, out string error)
    {
        error = string.Empty;

        if (String.IsNullOrEmpty(code))
            error = $"Invalid parameter: 'code': '{code}'.";
        else
        {
            if (!Guid.TryParse(state, out var authorizationRequestKey))
                error = $"Invalid authorization request key: {state}.";
            else
            {
                if (!s_authorizationRequests.TryGetValue(authorizationRequestKey, out var authRequest))
                    error = $"Authorization request {state} not found.";
                else if (!authRequest.IsPending)
                    error = $"Authorization request key {state} already used.";
                else
                    s_authorizationRequests[authorizationRequestKey].IsPending = false;
            }
        }

        return error == string.Empty;
    }

}

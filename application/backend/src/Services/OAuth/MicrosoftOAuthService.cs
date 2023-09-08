using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Web;
using backend.Model;
using backend.Model.Config;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Model.Users;
using backend.Services.Database;
using backend.Services.Users;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace backend.Services.OAuth;

public class MicrosoftOAuthService : IOAuthService
{
    private readonly ILogger<MicrosoftOAuthService> m_logger;
    private readonly DataContext m_context;
    private readonly OAuthConfig m_oauthConfig;
    private readonly DevOpsConfig m_devOpsConfig;
    private readonly IUserService m_userService;
    private readonly IHttpClientFactory m_httpClientFactory;

    private static readonly ConcurrentDictionary<Guid, TokenModel> s_authorizationRequests = new();

    public MicrosoftOAuthService(DataContext context, ILogger<MicrosoftOAuthService> logger, IOptions<OAuthConfig> oauthConfig, IUserService userService, IHttpClientFactory httpClientFactory, IOptions<DevOpsConfig> devOpsConfig)
    {
        m_context = context;
        m_logger = logger;
        m_oauthConfig = oauthConfig.Value;
        m_userService = userService;
        m_httpClientFactory = httpClientFactory;
        m_devOpsConfig = devOpsConfig.Value;
    }

    /// <summary>
    /// Get the url for a user to authorize this application to access the Azure DevOps API
    /// </summary>
    /// <returns>the url with queryparams</returns>
    public string GetNewAuthorizeUrl()
    {
        var state = Guid.NewGuid();
        s_authorizationRequests[state] = new TokenModel { IsPending = true };

        var uriBuilder = new UriBuilder(m_oauthConfig.AuthorizationUri);
        var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query ?? String.Empty);

        queryParams["client_id"] = m_oauthConfig.ClientId;
        queryParams["response_type"] = "Assertion";
        queryParams["state"] = state.ToString();
        queryParams["scope"] = m_oauthConfig.Scope;
        queryParams["redirect_uri"] = m_oauthConfig.RedirectUri;

        uriBuilder.Query = queryParams.ToString();

        return uriBuilder.ToString();
    }

    /// <summary>
    /// Handle callback of OAuth provider
    /// </summary>
    /// <param name="submission"></param>
    /// <returns>A token for the user to authenticate</returns>
    public async Task<AuthenticationResponse> HandleOAuthCallbackAsync(OAuthCallbackModel submission)
    {
        var code = submission.Code;
        var state = submission.State;
        m_logger.LogInformation($"Requested callback for auth request ${state}");
        foreach (var entry in s_authorizationRequests)
        {
            m_logger.LogInformation($"{entry.Key}: {entry.Value.IsPending}");
        }

        if (!CallbackValuesAreValid(code, state.ToString(), out var error))
        {
            m_logger.LogError($"Error when validating parameters: '{error}'");
            throw new BadRequestException(error);
        }

        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, m_oauthConfig.TokenUri);
        tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var form = new Dictionary<string, string>
        {
            { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
            { "client_assertion", m_oauthConfig.ClientSecret },
            { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
            { "assertion", code },
            { "redirect_uri", m_oauthConfig.RedirectUri }
        };

        tokenRequest.Content = new FormUrlEncodedContent(form);

        var response = await m_httpClientFactory.CreateClient().SendAsync(tokenRequest);

        if (!response.IsSuccessStatusCode)
        {
            m_logger.LogError($"Token request failed: {response}");
            var content = await response.Content.ReadAsStringAsync();
            m_logger.LogError($"Content: {content}");
            throw new BadRequestException($"Token request failed: {response.ReasonPhrase} {response.StatusCode}");
        }

        var body = await response.Content.ReadAsStringAsync();

        var token = s_authorizationRequests[state];
        JsonConvert.PopulateObject(body, token);
        var user = await GetUserInfoAndAddToken(token);

        return new AuthenticationResponse
        {
            User = UserResponse.From(user),
            Token = TokenResponse.From(token)
        };
    }

    private async Task<User> GetUserInfoAndAddToken(TokenModel token)
    {
        var connection = new VssConnection(new Uri(m_devOpsConfig.ServerUrl), new VssOAuthAccessTokenCredential(token.AccessToken));
        await connection.ConnectAsync();

        using var userClient = connection.GetClient<ProfileHttpClient>();
        var self = await userClient.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));
        var user = new User
        {
            Id = self.Id,
            DisplayName = self.DisplayName,
            EMail = self.EmailAddress,
        };

        await m_userService.CreateOrUpdateUserAsync(user);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = token.RefreshToken,
            IsActive = true
        });
        await m_context.SaveChangesAsync();
        return user;
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

        if (string.IsNullOrEmpty(code))
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

    public async Task<AuthenticationResponse> RefreshTokenAsync(string? token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, m_oauthConfig.TokenUri);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var form = new Dictionary<String, String>()
                {
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", m_oauthConfig.ClientSecret },
                    { "grant_type", "refresh_token" },
                    { "assertion", token },
                    { "redirect_uri", m_oauthConfig.RedirectUri }
                };
        requestMessage.Content = new FormUrlEncodedContent(form);

        var responseMessage = await m_httpClientFactory.CreateClient().SendAsync(requestMessage);

        if (responseMessage.IsSuccessStatusCode)
        {
            // Handle successful request
            var body = await responseMessage.Content.ReadAsStringAsync();
            var newToken = JObject.Parse(body).ToObject<TokenModel>();
            if (newToken == null)
            {
                throw new BadRequestException("Could not get new token.");
            }
            var user = await GetUserInfoAndAddToken(newToken);
            return new AuthenticationResponse
            {
                User = UserResponse.From(user),
                Token = TokenResponse.From(newToken)
            };
        }
        else
        {
            throw new BadRequestException(responseMessage.ReasonPhrase);
        }
    }
}

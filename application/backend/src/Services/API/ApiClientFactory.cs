using System.Net.Http.Headers;
using backend.Model.Config;
using backend.Model.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;

namespace backend.Services.API;

public class ApiClientFactory : IApiClientFactory
{
    private readonly DevOpsConfig _config;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<ApiClient> _logger;

    public ApiClientFactory(IOptions<DevOpsConfig> config, IHttpContextAccessor httpContextAccessor, ILogger<ApiClient> logger)
    {
        _config = config.Value;
        _contextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IApiClient> GetApiClientAsync(string accessToken)
    {
        _logger.LogDebug($"User with token ${accessToken} requested a client.");
        var connection = new VssConnection(new Uri(_config.ServerUrl), new VssOAuthAccessTokenCredential(accessToken));
        await connection.ConnectAsync();
        _logger.LogDebug($"Client for token ${accessToken} issued.");
        return new ApiClient(connection, _logger);
    }

    public async Task<IApiClient> GetApiClientAsync()
    {
        var token = _contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedException("No access token found in request, cannot create api client.");
        
        return await GetApiClientAsync(token);
    }
}
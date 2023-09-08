using System.Net.Http.Headers;
using backend.Model.Config;
using backend.Model.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;

namespace backend.Services.DevOps.Custom.API;

public class ApiClientFactory : IApiClientFactory
{
    private readonly DevOpsConfig m_config;
    private readonly IHttpContextAccessor m_contextAccessor;
    private readonly ILogger<ApiClient> m_logger;

    public ApiClientFactory(IOptions<DevOpsConfig> config, IHttpContextAccessor httpContextAccessor, ILogger<ApiClient> logger)
    {
        m_config = config.Value;
        m_contextAccessor = httpContextAccessor;
        m_logger = logger;
    }

    public async Task<IApiClient> GetApiClientAsync(string accessToken)
    {
        m_logger.LogDebug($"User with token ${accessToken} requested a client.");
        var connection = new VssConnection(new Uri(m_config.ServerUrl), new VssOAuthAccessTokenCredential(accessToken));
        await connection.ConnectAsync();
        m_logger.LogDebug($"Client for token ${accessToken} issued.");
        return new ApiClient(connection, m_logger);
    }

    public async Task<IApiClient> GetApiClientAsync()
    {
        var token = m_contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedException("No access token found in request, cannot create api client.");

        return await GetApiClientAsync(token);
    }
}
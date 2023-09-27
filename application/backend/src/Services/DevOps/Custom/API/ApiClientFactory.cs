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
        var connection = new VssConnection(new Uri(m_config.ServerUrl), new VssOAuthAccessTokenCredential(accessToken));
        await connection.ConnectAsync();
        return new ApiClient(connection, m_logger);
    }

    public async Task<IApiClient> GetApiClientAsync()
    {
        string? token = null;
        foreach (var method in m_config.TokenSources)
        {
            switch (method.Type)
            {
                case Model.Enum.TokenSourceType.Auth:
                    token = m_contextAccessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "");
                    break;
                case Model.Enum.TokenSourceType.Config:
                    token = method.Token;
                    break;
                case Model.Enum.TokenSourceType.Env:
                    if (string.IsNullOrEmpty(method.VariableName))
                        break;
                    token = Environment.GetEnvironmentVariable(method.VariableName);
                    break;
            }
            if (!string.IsNullOrEmpty(token))
                break;
        }

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedException($"No access token found, cannot create api client, tried ${m_config.TokenSources.Count} token sources [{string.Join(", ", m_config.TokenSources.Select(x => x.Type.ToString()))}].");

        return await GetApiClientAsync(token);
    }
}
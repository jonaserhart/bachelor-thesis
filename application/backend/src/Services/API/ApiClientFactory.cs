using backend.Model.Config;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;

namespace backend.Services.API;

public class ApiClientFactory : IApiClientFactory
{
    private readonly DevOpsConfig _config;

    public ApiClientFactory(IOptions<DevOpsConfig> config)
    {
        _config = config.Value;
    }

    public async Task<IApiClient> GetApiClientAsync(string accessToken)
    {
        var connection = new VssConnection(new Uri(_config.ServerUrl), new VssOAuthAccessTokenCredential(accessToken));
        await connection.ConnectAsync();
        return new ApiClient(connection);
    }
}
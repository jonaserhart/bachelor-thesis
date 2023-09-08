namespace backend.Services.DevOps.Custom.API;

public interface IApiClientFactory
{
    Task<IApiClient> GetApiClientAsync(string accessToken);
    Task<IApiClient> GetApiClientAsync();
}
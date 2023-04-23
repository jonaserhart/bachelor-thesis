namespace backend.Services.API;

public interface IApiClientFactory
{
    Task<IApiClient> GetApiClientAsync(string accessToken);
    Task<IApiClient> GetApiClientAsync();
}
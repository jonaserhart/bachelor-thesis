using backend.Model.Users;

namespace backend.Services.API;

public interface IApiClient
{
    Task<User> GetSelfAsync();
}
using System.Data;
using backend.Model.Users;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.Users.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace backend.Services.API;

public class ApiClient : IApiClient
{
    private readonly VssConnection _connection;

    public ApiClient(VssConnection connection)
    {
        _connection = connection;
    }

    public async Task<User> GetSelfAsync()
    {
        using var userClient = _connection.GetClient<ProfileHttpClient>();
        var self = await userClient.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));
        return new User
        {
            Id = self.Id.ToString(),
            DisplayName = self.DisplayName,
            EMail = self.EmailAddress,
        };
    }
}
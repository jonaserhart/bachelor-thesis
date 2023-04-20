using backend.Model.Users;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace backend.Services.API;

public interface IApiClient
{
    Task<User> GetSelfAsync();
    Task<IPagedList<TeamProjectReference>> GetProjectsAsync(int skip = 0);
}
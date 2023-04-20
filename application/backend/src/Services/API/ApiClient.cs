using System.Data;
using backend.Model.Users;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
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

    public async Task<IPagedList<TeamProjectReference>> GetProjectsAsync(int skip = 0)
    {
        using var projectsClient = _connection.GetClient<ProjectHttpClient>();
        var projects = await projectsClient.GetProjects(top: 100, skip: skip);
        return projects;
    }

    public async Task<IEnumerable<TeamSettingsIteration>> GetIterationsAsync(Guid projectId, Guid teamId)
    {
        using var workClient = _connection.GetClient<WorkHttpClient>();
        var iterations = await workClient.GetTeamIterationsAsync(new TeamContext(projectId, teamId));
        var i = iterations[0];
        return iterations;
    }

    public async Task<IterationWorkItems> GetIterationWorkItemsAsync(TeamContext teamContext, Guid iterationId)
    {
        using var workClient = _connection.GetClient<WorkHttpClient>();
        var workItems = await workClient.GetIterationWorkItemsAsync(teamContext, iterationId);
        return workItems;
    }
}
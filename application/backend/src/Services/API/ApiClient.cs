using System.Data;
using backend.Model.Analysis;
using backend.Model.Users;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace backend.Services.API;

public class ApiClient : IApiClient
{
    private readonly VssConnection _connection;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(VssConnection connection, ILogger<ApiClient> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<User> GetSelfAsync()
    {
        _logger.LogDebug($"Requested 'GetSelfAsync': for {_connection.AuthorizedIdentity.Descriptor}");
        using var userClient = _connection.GetClient<ProfileHttpClient>();
        var self = await userClient.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));
        return new User
        {
            Id = self.Id,
            DisplayName = self.DisplayName,
            EMail = self.EmailAddress,
        };
    }

    public async Task<IEnumerable<Project>> GetProjectsAsync(int skip = 0)
    {
        _logger.LogDebug($"Requested 'GeProjectsAsync': for {_connection.AuthorizedIdentity.Descriptor}");
        using var projectsClient = _connection.GetClient<ProjectHttpClient>();
        var projects = await projectsClient.GetProjects(top: 100, skip: skip);
        return projects.Select(Project.From);
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync(string projectId)
    {
        _logger.LogDebug($"Requested 'GetTeamsAsync': for {_connection.AuthorizedIdentity.Descriptor}");
        using var teamClient = _connection.GetClient<TeamHttpClient>();
        var teams = await teamClient.GetTeamsAsync(projectId);
        return teams.Select(Team.From);
    }

    public async Task<IEnumerable<Model.Analysis.FieldInfo>> GetWiqlFieldsAsync(Query wiql, string iterationPath)
    {
        _logger.LogDebug($"Requested 'GetWiqlFieldsAsync': for {_connection.AuthorizedIdentity.Descriptor}");
        using var witClient = _connection.GetClient<WorkItemTrackingHttpClient>();
        var info = await witClient.QueryByWiqlAsync(new Wiql
        {
            Query = wiql.ToQuery(iterationPath)
        });

        var ids = info.WorkItems.Select(x => x.Id);
        var fields = info.Columns.Select(x => x.ReferenceName);

        var fieldInfos = new List<Model.Analysis.FieldInfo>();
        foreach(var field in fields)
        {
            var fieldInfo = await witClient.GetFieldAsync(field);
            fieldInfos.Add(Model.Analysis.FieldInfo.From(fieldInfo));
        } 
        return fieldInfos;
    }

    public async Task<IEnumerable<Iteration>> GetIterationsAsync(string projectId, string teamId)
    {
        using var workClient = _connection.GetClient<WorkHttpClient>();
        var iterations = await workClient.GetTeamIterationsAsync(new TeamContext(projectId, teamId));
        return iterations.Select(Iteration.From);
    }

    public async Task<IterationWorkItems> GetIterationWorkItemsAsync(TeamContext teamContext, Guid iterationId)
    {
        using var workClient = _connection.GetClient<WorkHttpClient>();
        var workItems = await workClient.GetIterationWorkItemsAsync(teamContext, iterationId);
        return workItems;
    }
}
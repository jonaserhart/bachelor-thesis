using System.Data;
using backend.Model.Analysis;
using backend.Model.Custom;
using backend.Model.Users;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace backend.Services.API;

public sealed class ApiClient : IApiClient
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
    public async Task<IEnumerable<AzureDevopsIteration>> GetIterationsAsync(string projectId, string teamId)
    {
        using var workClient = _connection.GetClient<WorkHttpClient>();
        var iterations = await workClient.GetTeamIterationsAsync(new TeamContext(projectId, teamId));
        return iterations.Select(x => new AzureDevopsIteration { Id = x.Id, Name = x.Name, Path = x.Path });
    }

    public async Task<List<Dictionary<string, object>>> GetIterationTasksAsync(TeamContext teamContext, Guid iterationId)
    {
        var workItems = await GetIterationWorkItemsAsync(teamContext, iterationId);

        var tasks = workItems.WorkItemRelations.Where(x => x.Rel == "System.LinkTypes.Hierarchy-Forward").Select(x => x.Target.Id);

        var fetchedWorkItems = new List<Dictionary<string, object>>();

        using var witClient = await _connection.GetClientAsync<WorkItemTrackingHttpClient>();
        foreach (var id in tasks)
        {
            var workItem = await witClient.GetWorkItemAsync(teamContext.Project ?? teamContext.ProjectId.ToString(), id);
            var wi = workItem.Fields.ToDictionary(x => x.Key, x => x.Value);
            fetchedWorkItems.Add(wi);
        }

        return fetchedWorkItems;
    }

    public async Task<List<Dictionary<string, object>>> GetIterationTicketsAsync(TeamContext teamContext, Guid iterationId)
    {
        var workItems = await GetIterationWorkItemsAsync(teamContext, iterationId);

        var userStoriesAndBugs = workItems.WorkItemRelations.Where(x => x.Rel == null).Select(x => x.Target.Id);

        var fetchedWorkItems = new List<Dictionary<string, object>>();

        using var witClient = await _connection.GetClientAsync<WorkItemTrackingHttpClient>();
        foreach (var id in userStoriesAndBugs)
        {
            var workItem = await witClient.GetWorkItemAsync(teamContext.Project ?? teamContext.ProjectId.ToString(), id);
            var wi = workItem.Fields.ToDictionary(x => x.Key, x => x.Value); ;
            fetchedWorkItems.Add(wi);
        }

        return fetchedWorkItems;
    }

    private async Task<IterationWorkItems> GetIterationWorkItemsAsync(TeamContext teamContext, Guid iterationId)
    {
        using var workClient = _connection.GetClient<WorkHttpClient>();
        var workItems = await workClient.GetIterationWorkItemsAsync(teamContext, iterationId);

        return workItems;
    }

    public async Task<double> GetIterationCapacities(string project, Guid iteration)
    {
        using var workClient = _connection.GetClient<WorkHttpClient>();
        var capacity = await workClient.GetTotalIterationCapacitiesAsync(project, iteration);

        return capacity.TotalIterationCapacityPerDay;
    }

    public void Dispose() => _connection.Dispose();
}
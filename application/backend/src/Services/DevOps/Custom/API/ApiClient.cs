using System.Security.Cryptography.X509Certificates;
using System.Data;
using backend.Model.Analysis;
using backend.Model.Custom;
using backend.Model.Users;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace backend.Services.DevOps.Custom.API;

public sealed class ApiClient : IApiClient
{
    private readonly VssConnection m_connection;
    private readonly ILogger<ApiClient> m_logger;

    public ApiClient(VssConnection connection, ILogger<ApiClient> logger)
    {
        m_connection = connection;
        m_logger = logger;
    }

    public async Task<IEnumerable<(Guid Id, string Name)>> GetProjectNameAsync()
    {
        var client = m_connection.GetClient<ProjectHttpClient>();
        var projects = await client.GetProjects();
        return projects.Select(x => (x.Id, x.Name));
    }

    public async Task<IEnumerable<AzureDevopsIteration>> GetIterationsAsync(string projectId, string teamId)
    {
        var workClient = m_connection.GetClient<WorkHttpClient>();
        var iterations = await workClient.GetTeamIterationsAsync(new TeamContext(projectId, teamId));
        return iterations.Select(x => new AzureDevopsIteration { Id = x.Id, Name = x.Name, Path = x.Path });
    }

    public async Task<List<Dictionary<string, object>>> GetIterationTasksAsync(TeamContext teamContext, Guid iterationId)
    {
        var workItems = await GetIterationWorkItemsAsync(teamContext, iterationId);

        var tasks = workItems.WorkItemRelations.Where(x => x.Rel == "System.LinkTypes.Hierarchy-Forward").Select(x => x.Target.Id);

        var fetchedWorkItems = new List<Dictionary<string, object>>();

        var witClient = await m_connection.GetClientAsync<WorkItemTrackingHttpClient>();
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

        var witClient = await m_connection.GetClientAsync<WorkItemTrackingHttpClient>();
        foreach (var id in userStoriesAndBugs)
        {
            var workItem = await witClient.GetWorkItemAsync(teamContext.Project ?? teamContext.ProjectId.ToString(), id);
            var wi = workItem.Fields.ToDictionary(x => x.Key, x => x.Value); ;
            fetchedWorkItems.Add(wi);
        }

        return fetchedWorkItems;
    }

    public async Task<List<Dictionary<string, object>>> GetRemovedWorkItemsAsync(string project, string iterationPath, DateTime sprintEnd, DateTime sprintStart, params string[] workItemTypes)
    {
        var itemsAtStart = await GetWorkItemsOfIterationAndAsOfAsync(project, iterationPath, sprintStart, workItemTypes);
        var itemsAtEnd = await GetWorkItemsOfIterationAndAsOfAsync(project, iterationPath, sprintEnd, workItemTypes);

        var removedItems = itemsAtStart.Except(itemsAtEnd, new WorkItemFieldsComparer()).ToList();
        return removedItems;
    }

    public async Task<List<Dictionary<string, object>>> GetWorkItemsOfIterationAndAsOfAsync(string project, string iterationPath, DateTime asOf, params string[] workitemTypes)
    {
        if (workitemTypes.Count() <= 0)
        {
            throw new ArgumentException("Argument 'workItemTypes' cannot be empty.");
        }

        var queryParts = workitemTypes.Select(type => $"[System.WorkItemType] = '{type}'").ToArray();

        var workItemTypeClause = string.Join(" OR ", queryParts);

        var query = $"""
            SELECT [System.Id]
            FROM workitems 
            WHERE 
                [System.TeamProject] = '{project}' 
                AND ({workItemTypeClause}) 
                AND (EVER ([System.IterationPath] = '{iterationPath}' ))
        """;

        var witClient = await m_connection.GetClientAsync<WorkItemTrackingHttpClient>();

        var queryResult = await witClient.QueryByWiqlAsync(new Wiql() { Query = query });
        var ids = queryResult.WorkItems.Select(x => x.Id);

        var fetchedWorkItems = new List<Dictionary<string, object>>();

        foreach (var id in ids)
        {
            try
            {
                var workItem = await witClient.GetWorkItemAsync(project, id, expand: WorkItemExpand.All, asOf: asOf);
                if (workItem.Fields.TryGetValue("System.IterationPath", out var actualIteration) && iterationPath.Equals(actualIteration?.ToString()))
                {
                    var wi = workItem.Fields.ToDictionary(x => x.Key, x => x.Value);
                    wi.TryAdd("System.Id", workItem.Id);
                    fetchedWorkItems.Add(wi);
                }
            }
            catch (Exception e)
            {
                m_logger.LogError(e.Message);
            }
        }

        return fetchedWorkItems;
    }

    public async Task<List<Dictionary<string, object>>> GetNormalTicketsAsOfAsync(string project, string iterationPath, DateTime asOf)
    {
        var witClient = await m_connection.GetClientAsync<WorkItemTrackingHttpClient>();

        var query = $"""
            SELECT [System.Id] 
            FROM workitems 
            WHERE 
                [System.TeamProject] = '{project}' 
                AND [System.WorkItemType] = 'Task' 
                AND [LatitudeAgile.Blocker] = false
                AND [Custom.8edfbb8b-fae7-4ede-8694-d157a4d38e8a] = false
                AND (EVER ([System.IterationPath] = '{iterationPath}' ))
        """;

        var queryResult = await witClient.QueryByWiqlAsync(new Wiql() { Query = query });
        var ids = queryResult.WorkItems.Select(x => x.Id);

        var fetchedWorkItems = await GetParentWorkItems(project, asOf, witClient, ids, iterationPath);

        return fetchedWorkItems;
    }

    public async Task<List<Dictionary<string, object>>> GetBlockerTicketsAsOfAsync(string project, string iterationPath, DateTime asOf)
    {
        var witClient = await m_connection.GetClientAsync<WorkItemTrackingHttpClient>();

        var query = $"""
            SELECT [System.Id] 
            FROM workitems 
            WHERE 
                [System.TeamProject] = '{project}' 
                AND [System.WorkItemType] = 'Task' 
                AND [LatitudeAgile.Blocker] = true
                AND (EVER ([System.IterationPath] = '{iterationPath}' ))
        """;

        var queryResult = await witClient.QueryByWiqlAsync(new Wiql() { Query = query });
        var ids = queryResult.WorkItems.Select(x => x.Id);

        var fetchedWorkItems = await GetParentWorkItems(project, asOf, witClient, ids, iterationPath);

        return fetchedWorkItems;
    }

    public async Task<List<Dictionary<string, object>>> GetAfterThoughtTicketsAsOfAsync(string project, string iterationPath, DateTime asOf)
    {
        var witClient = await m_connection.GetClientAsync<WorkItemTrackingHttpClient>();

        var query = $"""
            SELECT [System.Id] 
            FROM workitems 
            WHERE 
                [System.TeamProject] = '{project}' 
                AND [System.WorkItemType] = 'Task' 
                AND [Custom.8edfbb8b-fae7-4ede-8694-d157a4d38e8a] = true
                AND (EVER ([System.IterationPath] = '{iterationPath}' ))
        """;

        var queryResult = await witClient.QueryByWiqlAsync(new Wiql() { Query = query });
        var ids = queryResult.WorkItems.Select(x => x.Id);
        var fetchedWorkItems = await GetParentWorkItems(project, asOf, witClient, ids, iterationPath);

        return fetchedWorkItems;
    }

    private async Task<List<Dictionary<string, object>>> GetParentWorkItems(string project, DateTime asOf, WorkItemTrackingHttpClient witClient, IEnumerable<int> ids, string iterationPath)
    {
        var fetchedWorkItems = new Dictionary<int, Dictionary<string, object>>();

        foreach (var id in ids)
        {
            try
            {
                var workItem = await witClient.GetWorkItemAsync(project, id, expand: WorkItemExpand.All, asOf: asOf);
                var parent = workItem.Fields.Where(x => x.Key == "System.Parent").FirstOrDefault();
                if (int.TryParse(parent.Value?.ToString(), out var parentId))
                {
                    if (fetchedWorkItems.ContainsKey(parentId))
                    {
                        continue;
                    }
                    var parentWorkItem = await witClient.GetWorkItemAsync(project, parentId, expand: WorkItemExpand.Fields, asOf: asOf);
                    if (parentWorkItem.Fields.TryGetValue("System.IterationPath", out var actualIteration) && iterationPath.Equals(actualIteration?.ToString()))
                    {
                        var wi = parentWorkItem.Fields.ToDictionary(x => x.Key, x => x.Value);
                        wi.TryAdd("System.Id", parentId);
                        fetchedWorkItems.Add(parentId, wi);
                    }

                }
            }
            catch (Exception e)
            {
                m_logger.LogError(e.Message);
            }
        }

        return fetchedWorkItems.Values.ToList();
    }

    private async Task<IterationWorkItems> GetIterationWorkItemsAsync(TeamContext teamContext, Guid iterationId)
    {
        var workClient = m_connection.GetClient<WorkHttpClient>();
        var workItems = await workClient.GetIterationWorkItemsAsync(teamContext, iterationId);

        return workItems;
    }

    public int GetWorkdaysOff(DateTime startDate, DateTime endDate, DayOfWeek[] nonWorkDays)
    {
        var workDaysOff = 0;
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            if (!nonWorkDays.Contains(currentDate.DayOfWeek))
            {
                workDaysOff++;
            }
            currentDate = currentDate.AddDays(1);
        }

        return workDaysOff;
    }

    public async Task<double> GetIterationCapacitiesAsync(TeamContext teamContext, Guid iteration)
    {
        var workClient = m_connection.GetClient<WorkHttpClient>();
        var r = await workClient.GetCapacitiesWithIdentityRefAndTotalsAsync(teamContext, iteration);

        var nonWorkDays = new[] { DayOfWeek.Sunday, DayOfWeek.Saturday };

        var capacity = 0.0;
        var sprintWorkDays = 10;

        foreach (var teamMember in r.TeamMembers)
        {
            var numWorkDaysOff = teamMember.DaysOff.Sum(x => GetWorkdaysOff(x.Start, x.End, nonWorkDays));
            var devCapacity = teamMember.Activities.FirstOrDefault(x => x.Name == "Development");
            if (devCapacity != null)
            {
                capacity += devCapacity.CapacityPerDay * (sprintWorkDays - numWorkDaysOff);
            }
        }

        return capacity;
    }

    public void Dispose() => m_connection.Dispose();
}
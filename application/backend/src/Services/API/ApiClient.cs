using System.Data;
using backend.Model.Analysis;
using backend.Model.Analysis.WorkItems;
using backend.Model.Enum;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Model.Users;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;
using FieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType;

namespace backend.Services.API;

public sealed class ApiClient : IApiClient
{
    private readonly VssConnection _connection;
    private readonly ILogger<ApiClient> _logger;
    private bool m_disposedValue;

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

    public async Task<IEnumerable<Model.Analysis.FieldInfo>> GetFieldInfosAsync(string projectId)
    {
        _logger.LogDebug($"Requested 'GetFieldInfo': for {_connection.AuthorizedIdentity.Descriptor}");
        using var witClient = _connection.GetClient<WorkItemTrackingHttpClient>();
        var infos = await witClient.GetFieldsAsync(projectId);

        var fieldInfos = infos.Select(FieldInfoFrom);

        _logger.LogDebug($"Got field infos: {fieldInfos.Select(x => $"\n\t{x.ToString()}")}");

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

    public async Task<IEnumerable<QueryResponse>> GetQueriesAsync(string projectId)
    {
        using var workItemTrackingHttpClient = _connection.GetClient<WorkItemTrackingHttpClient>();
        var queries = await workItemTrackingHttpClient.GetQueriesAsync(projectId, depth: 2);
        return queries.Select(QueryResponse.From);
    }

    public async Task<Query> GetQueryByIdAsync(string projectId, string queryId)
    {
        using var workItemTrackingHttpClient = _connection.GetClient<WorkItemTrackingHttpClient>();
        var query = await workItemTrackingHttpClient.GetQueryAsync(projectId, queryId, QueryExpand.All);
        if (query.HasChildren.HasValue && query.HasChildren.Value)
        {
            throw new BadRequestException($"Query with id {queryId} is a folder, not a query.");
        }

        var result = Query.From(query);
        var fieldInfos = new List<Model.Analysis.FieldInfo>();
        foreach (var field in query.Columns)
        {
            var info = await workItemTrackingHttpClient.GetFieldAsync(field.ReferenceName);
            var fieldInfo = FieldInfoFrom(info);
            fieldInfos.Add(fieldInfo);
        }
        result.Select = fieldInfos;

        return result;
    }

    private async IAsyncEnumerable<WorkItem> FetchWorkItemsAsync(WorkItemTrackingHttpClient workItemTrackingHttpClient,
                                                                 IEnumerable<int> ids,
                                                                 string projectId,
                                                                 Iteration iteration,
                                                                 Query wiql,
                                                                 DateTime? asOf)
    {
        foreach (var id in ids)
        {
            var item = await workItemTrackingHttpClient.GetWorkItemAsync(projectId, id, wiql.Select.Select(x => x.ReferenceName), asOf);
            yield return item;
        }
    }

    public async Task<IEnumerable<Workitem>> GetWorkitemsAsync(string projectId, Iteration iteration, Query wiql, DateTime? asOf)
    {
        using var workItemTrackingHttpClient = _connection.GetClient<WorkItemTrackingHttpClient>();
        var itemRefs = await workItemTrackingHttpClient.QueryByWiqlAsync(new Wiql { Query = wiql.ToQuery(iteration.Path, asOf) });

        var idsToFetch = itemRefs.WorkItems.Select(x => x.Id);

        var workItems = new List<Workitem>();

        await foreach (var item in FetchWorkItemsAsync(workItemTrackingHttpClient, idsToFetch, projectId, iteration, wiql, asOf))
        {
            var workItem = new Workitem();

            foreach (var selectedField in wiql.Select)
            {
                var key = selectedField.ReferenceName;
                if (!item.Fields.TryGetValue(key, out var fieldValue))
                {
                    _logger.LogError($"Could not get field from wiql (query: ${wiql.Id}, field: ${key})");
                    continue;
                }
                workItem.WorkItemFields.Add(new WorkItemKeyValue { Key = key, Type = selectedField.Type, Value = fieldValue?.ToString() ?? string.Empty });
            }
            workItems.Add(workItem);
        }
        return workItems;
    }

    // Conversion
    public static Model.Analysis.FieldInfo FieldInfoFrom(WorkItemField field)
    {
        return new Model.Analysis.FieldInfo
        {
            Name = field.Name,
            ReferenceName = field.ReferenceName,
            Type = field.Type switch
            {
                FieldType.Guid
                or FieldType.Html
                or FieldType.PicklistString
                or FieldType.PlainText
                or FieldType.TreePath
                or FieldType.String => WorkItemValueType.String,
                FieldType.Double
                or FieldType.Integer
                or FieldType.PicklistDouble
                or FieldType.PicklistInteger => WorkItemValueType.Number,
                FieldType.Boolean => WorkItemValueType.Boolean,
                _ => WorkItemValueType.Unknown
            },
        };
    }

    public void Dispose() => _connection.Dispose();
}
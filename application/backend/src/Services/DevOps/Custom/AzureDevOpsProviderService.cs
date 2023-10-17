using System.Runtime.CompilerServices;
using backend.Model.Analysis;
using backend.Model.Config;
using backend.Model.Exceptions;
using backend.Services.DevOps.Custom.API;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Core.WebApi.Types;

namespace backend.Services.DevOps.Custom;

public class AzureDevOpsProviderService : IDevOpsProviderService
{
    private readonly IApiClientFactory m_apiClientFactory;
    private readonly List<Query> m_queries;
    // Query Ids
    private const string LAST_ITER_WI_TICKETS_QUERY = "LAST_ITER_WI_TICKETS_QUERY";
    private const string LAST_ITER_WI_TASKS_QUERY = "LAST_ITER_WI_TASKS_QUERY";
    private const string LAST_ITER_CAP_QUERY = "LAST_ITER_CAP_QUERY";
    private const string LAST_ITER_START_WI_TICKETS_QUERY = "LAST_ITER_START_WI_TICKETS_QUERY";
    private const string LAST_ITER_END_WI_TICKETS_QUERY = "LAST_ITER_END_WI_TICKETS_QUERY";
    private const string LAST_ITER_START_WI_TASKS_QUERY = "LAST_ITER_START_WI_TASKS_QUERY";
    private const string LAST_ITER_END_WI_TASKS_QUERY = "LAST_ITER_END_WI_TASKS_QUERY";
    private const string LAST_ITER_WI_REMOVED_TICKETS_QUERY = "LAST_ITER_END_WI_REMOVED_TICKETS_QUERY";
    private const string LAST_ITER_WI_TICKET_HAS_BLOCKERS_QUERY = "LAST_ITER_WI_TICKET_HAS_BLOCKERS_QUERY";
    private const string LAST_ITER_WI_TICKET_HAS_AFTERTHOUGHTTASKS_QUERY = "LAST_ITER_WI_TICKET_HAS_AFTERTHOUGHTTASKS_QUERY";

    // Parameter names
    private const string ITERATION_PARAMETER = "iteration";
    private const string ITERATION_PATH_PARAMETER = "iterationPath";
    private const string SPRINT_START_PARAMETER = "sprintStart";
    private const string SPRINT_END_PARAMETER = "sprintEnd";
    private readonly string? m_project;
    private readonly string? m_devTeam;
    public AzureDevOpsProviderService(IApiClientFactory apiClientFactory, IOptions<DevOpsConfig> options)
    {
        var serviceConfig = options.Value.ServiceConfiguration;
        if (serviceConfig.TryGetValue("project", out var proj))
        {
            m_project = proj?.ToString();
        }
        if (serviceConfig.TryGetValue("team", out var devTeam))
        {
            m_devTeam = devTeam?.ToString();
        }

        m_apiClientFactory = apiClientFactory;
        m_queries = new List<Query>
        {
            new(LAST_ITER_WI_TASKS_QUERY, "Last iteration tasks", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_WI_TICKETS_QUERY, "Last iteration tickets", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_CAP_QUERY, "Last iteration capacity", Model.Enum.QueryReturnType.Number),
            new(LAST_ITER_START_WI_TICKETS_QUERY, "Tickets at sprint start", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_END_WI_TICKETS_QUERY, "Tickets at sprint end", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_START_WI_TASKS_QUERY, "Tasks at sprint start", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_END_WI_TASKS_QUERY, "Tasks at sprint end", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_WI_REMOVED_TICKETS_QUERY, "Removed tickets", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_WI_TICKET_HAS_BLOCKERS_QUERY, "Tickets with blocker tasks", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_WI_TICKET_HAS_AFTERTHOUGHTTASKS_QUERY, "Tickets with afterthought tasks", Model.Enum.QueryReturnType.ObjectList),
        };
    }

    public async Task<List<Query>> GetQueries()
    {
        var queryList = m_queries.ToList();
        using var client = await m_apiClientFactory.GetApiClientAsync();

        foreach (var q in queryList.Where(x => x.Type == Model.Enum.QueryReturnType.ObjectList || x.Type == Model.Enum.QueryReturnType.Object))
        {
            var fields = await client.GetAvailableFields(m_project!);
            q.AdditionalQueryData.Add("possibleFields", fields);
        }

        return queryList;
    }
    public Query GetQueryById(string id)
    {
        var query = m_queries.FirstOrDefault(x => x.Id == id) ?? throw new DbKeyNotFoundException(id, typeof(Query));
        return query;
    }

    private async Task AddIterationParameter(List<QueryParameter> queryParameters, IApiClient client)
    {
        if (string.IsNullOrEmpty(m_project) || string.IsNullOrEmpty(m_devTeam))
        {
            throw new QueryExecuteException("Azure DevOps service was not configured properly, cannot get iterations.");
        }

        var iterations = await client.GetIterationsAsync(m_project, m_devTeam);
        queryParameters.Add(new QueryParameter
        {
            Name = ITERATION_PARAMETER,
            DisplayName = "Iteration (Sprint)",
            Description = "Iteration (Sprint) to run the query on",
            Type = Model.Enum.QueryParameterValueType.Select,
            Data = iterations?.Select(x => new { label = $"{x.Name} ({x.Path})", value = x.Id }).ToArray()
        });
    }

    private async Task AddIterationPathParameter(List<QueryParameter> queryParameters, IApiClient client)
    {
        if (string.IsNullOrEmpty(m_project) || string.IsNullOrEmpty(m_devTeam))
        {
            throw new QueryExecuteException("Azure DevOps service was not configured properly, cannot get iterations.");
        }

        var iterations = await client.GetIterationsAsync(m_project, m_devTeam);
        queryParameters.Add(new QueryParameter
        {
            Name = ITERATION_PATH_PARAMETER,
            DisplayName = "Iteration Path (Sprint)",
            Description = "Iteration Path / Sprint to run the query on",
            Type = Model.Enum.QueryParameterValueType.Select,
            Data = iterations?.Select(x => new { label = $"{x.Path}", value = x.Path }).ToArray()
        });
    }

    private void AddStartDateParameter(List<QueryParameter> queryParameters)
    {
        queryParameters.Add(new QueryParameter
        {
            Name = SPRINT_START_PARAMETER,
            DisplayName = "Start of sprint",
            Description = "The start of the Sprint",
            Type = Model.Enum.QueryParameterValueType.Date,
        });
    }

    private void AddEndDateParameter(List<QueryParameter> queryParameters)
    {
        queryParameters.Add(new QueryParameter
        {
            Name = SPRINT_END_PARAMETER,
            DisplayName = "End of sprint",
            Description = "The end of the Sprint",
            Type = Model.Enum.QueryParameterValueType.Date,
        });
    }

    public async Task<List<QueryParameter>> GetQueryRuntimeParametersAsync(string queryId)
    {
        var query = m_queries.FirstOrDefault(x => x.Id == queryId) ?? throw new KeyNotFoundException();
        var queryParameters = new List<QueryParameter>();

        using var client = await m_apiClientFactory.GetApiClientAsync();
        switch (query.Id)
        {
            case LAST_ITER_WI_TASKS_QUERY:
            case LAST_ITER_WI_TICKETS_QUERY:
            case LAST_ITER_CAP_QUERY:
                await AddIterationParameter(queryParameters, client);
                break;
            case LAST_ITER_START_WI_TICKETS_QUERY:
            case LAST_ITER_START_WI_TASKS_QUERY:
                await AddIterationPathParameter(queryParameters, client);
                AddStartDateParameter(queryParameters);
                break;
            case LAST_ITER_END_WI_TICKETS_QUERY:
            case LAST_ITER_END_WI_TASKS_QUERY:
                await AddIterationPathParameter(queryParameters, client);
                AddEndDateParameter(queryParameters);
                break;
            case LAST_ITER_WI_REMOVED_TICKETS_QUERY:
                await AddIterationPathParameter(queryParameters, client);
                AddStartDateParameter(queryParameters);
                AddEndDateParameter(queryParameters);
                break;
            case LAST_ITER_WI_TICKET_HAS_BLOCKERS_QUERY:
                await AddIterationPathParameter(queryParameters, client);
                AddEndDateParameter(queryParameters);
                break;
            case LAST_ITER_WI_TICKET_HAS_AFTERTHOUGHTTASKS_QUERY:
                await AddIterationPathParameter(queryParameters, client);
                AddEndDateParameter(queryParameters);
                break;
            default:
                throw new KeyNotFoundException($"Query with id {query.Id} not found.");
        }
        return queryParameters ?? new List<QueryParameter>();
    }

    public async Task<QueryResult> ExecuteQueryAsync(string queryId, Dictionary<string, object?> parameterValues)
    {
        if (string.IsNullOrEmpty(m_project) || string.IsNullOrEmpty(m_devTeam))
        {
            throw new QueryExecuteException("Azure DevOps service was not configured properly, cannot execute queries.");
        }

        var query = m_queries.FirstOrDefault(x => x.Id == queryId) ?? throw new KeyNotFoundException();
        using var apiClient = await m_apiClientFactory.GetApiClientAsync();

        object? value;

        // Possible arguments
        Guid iterationId;
        string iterationPath;
        DateTime sprintStart;
        DateTime sprintEnd;

        var result = new QueryResult
        {
            Type = query.Type
        };

        switch (query.Id)
        {
            case LAST_ITER_WI_TASKS_QUERY:
                iterationId = GetGuidParameterOrThrow(ITERATION_PARAMETER, parameterValues);

                var tasks = await apiClient.GetIterationTasksAsync(new TeamContext(m_project, m_devTeam), iterationId);
                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PARAMETER, Value = iterationId });
                value = tasks;
                break;
            case LAST_ITER_WI_TICKETS_QUERY:
                iterationId = GetGuidParameterOrThrow(ITERATION_PARAMETER, parameterValues);

                var userStoriesAndBugs = await apiClient.GetIterationTicketsAsync(new TeamContext(m_project, m_devTeam), iterationId);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PARAMETER, Value = iterationId });
                value = userStoriesAndBugs;
                break;
            case LAST_ITER_CAP_QUERY:
                iterationId = GetGuidParameterOrThrow(ITERATION_PARAMETER, parameterValues);
                var capacity = await apiClient.GetIterationCapacitiesAsync(new TeamContext(m_project, m_devTeam), iterationId);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PARAMETER, Value = iterationId });
                value = double.Round(capacity, 2);
                break;
            case LAST_ITER_START_WI_TICKETS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintStart = GetDateTimeParameterOrThrow(SPRINT_START_PARAMETER, parameterValues);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PATH_PARAMETER, Value = iterationPath });
                value = await apiClient.GetNormalTicketsAsOfAsync(m_project, iterationPath, sprintStart);
                break;
            case LAST_ITER_START_WI_TASKS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintStart = GetDateTimeParameterOrThrow(SPRINT_START_PARAMETER, parameterValues);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PATH_PARAMETER, Value = iterationPath });
                result.ParameterValues.Add(new QueryParameterValue { Name = SPRINT_START_PARAMETER, Value = sprintStart });
                value = await apiClient.GetWorkItemsOfIterationAndAsOfAsync(m_project, iterationPath, sprintStart, "Task");
                break;
            case LAST_ITER_END_WI_TICKETS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PATH_PARAMETER, Value = iterationPath });
                result.ParameterValues.Add(new QueryParameterValue { Name = SPRINT_END_PARAMETER, Value = sprintEnd });
                value = await apiClient.GetNormalTicketsAsOfAsync(m_project, iterationPath, sprintEnd);
                break;
            case LAST_ITER_END_WI_TASKS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PATH_PARAMETER, Value = iterationPath });
                result.ParameterValues.Add(new QueryParameterValue { Name = SPRINT_END_PARAMETER, Value = sprintEnd });
                value = await apiClient.GetWorkItemsOfIterationAndAsOfAsync(m_project, iterationPath, sprintEnd, "Task");
                break;
            case LAST_ITER_WI_TICKET_HAS_BLOCKERS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PATH_PARAMETER, Value = iterationPath });
                result.ParameterValues.Add(new QueryParameterValue { Name = SPRINT_END_PARAMETER, Value = sprintEnd });
                value = await apiClient.GetBlockerTicketsAsOfAsync(m_project, iterationPath, sprintEnd);
                break;
            case LAST_ITER_WI_TICKET_HAS_AFTERTHOUGHTTASKS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PATH_PARAMETER, Value = iterationPath });
                result.ParameterValues.Add(new QueryParameterValue { Name = SPRINT_END_PARAMETER, Value = sprintEnd });
                value = await apiClient.GetAfterThoughtTicketsAsOfAsync(m_project, iterationPath, sprintEnd);
                break;
            case LAST_ITER_WI_REMOVED_TICKETS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintStart = GetDateTimeParameterOrThrow(SPRINT_START_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                result.ParameterValues.Add(new QueryParameterValue { Name = ITERATION_PATH_PARAMETER, Value = iterationPath });
                result.ParameterValues.Add(new QueryParameterValue { Name = SPRINT_START_PARAMETER, Value = sprintStart });
                result.ParameterValues.Add(new QueryParameterValue { Name = SPRINT_END_PARAMETER, Value = sprintEnd });
                value = await apiClient.GetRemovedWorkItemsAsync(m_project, iterationPath, sprintEnd, sprintStart, "Task");
                break;
            default:
                throw new KeyNotFoundException($"Query with id {query.Id} not found.");
        }

        result.Value = value;

        return result;
    }

    private object? GetParameterFromValuesOrThrow(string paramName, Dictionary<string, object?> parameterValues)
    {
        if (!parameterValues.TryGetValue(paramName, out var parameter))
            throw new QueryExecuteException($"Could not find parameter {paramName} in given parameters: Found keys: ${string.Join(',', parameterValues.Keys)}");
        return parameter;
    }

    private Guid GetGuidParameterOrThrow(string paramName, Dictionary<string, object?> parameterValues)
    {
        var parameter = GetParameterFromValuesOrThrow(paramName, parameterValues);
        if (parameter == null
            || !Guid.TryParse(parameter.ToString(), out var iterationId))
            throw new QueryExecuteException($"Parameter {paramName} is supposed to be a Guid, but was {parameter}");
        return iterationId;
    }

    private string GetStringParameterOrThrow(string paramName, Dictionary<string, object?> parameterValues)
    {
        var parameter = GetParameterFromValuesOrThrow(paramName, parameterValues);
        if (string.IsNullOrEmpty(parameter?.ToString()))
            throw new QueryExecuteException($"Parameter {paramName} was supposed to be a non-empty string, but was: {parameter}");

        return parameter.ToString() ?? string.Empty;
    }

    private DateTime GetDateTimeParameterOrThrow(string paramName, Dictionary<string, object?> parameterValues)
    {
        var parameter = GetParameterFromValuesOrThrow(paramName, parameterValues);
        if (!DateTime.TryParse(parameter?.ToString(), out var result))
            throw new QueryExecuteException($"Parameter {paramName} was supposed to be a datetime, but was: {parameter}");

        return result;
    }

    public bool HasValidConfiguration() =>
            !string.IsNullOrEmpty(m_project) && !string.IsNullOrEmpty(m_devTeam);
}
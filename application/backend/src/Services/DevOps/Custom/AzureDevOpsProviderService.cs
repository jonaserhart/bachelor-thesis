using System;
using System.Security.Cryptography.X509Certificates;
using backend.Model.Analysis;
using backend.Model.Exceptions;
using backend.Services.DevOps.Custom.API;
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
    private const string GEOMAN_PROJECT = "GeoMan";
    private const string DEV_TEAM = "0_DEV-Team 1";
    public AzureDevOpsProviderService(IApiClientFactory apiClientFactory)
    {
        m_apiClientFactory = apiClientFactory;
        m_queries = new List<Query>
        {
            new(LAST_ITER_WI_TASKS_QUERY, "GeoMan - Last iteration tasks", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_WI_TICKETS_QUERY, "GeoMan - Last iteration tickets", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_CAP_QUERY, "GeoMan - Last iteration capacity", Model.Enum.QueryReturnType.Number),
            new(LAST_ITER_START_WI_TICKETS_QUERY, "GeoMan - Tickets at sprint start", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_END_WI_TICKETS_QUERY, "GeoMan - Tickets at sprint end", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_START_WI_TASKS_QUERY, "GeoMan - Tasks at sprint start", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_END_WI_TASKS_QUERY, "GeoMan - Tasks at sprint end", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_WI_REMOVED_TICKETS_QUERY, "GeoMan - Removed tickets", Model.Enum.QueryReturnType.ObjectList),
            new(LAST_ITER_WI_TICKET_HAS_BLOCKERS_QUERY, "GeoMan - Tickets with blocker tasks", Model.Enum.QueryReturnType.ObjectList),
        };
    }

    public List<Query> GetQueries() => m_queries;
    public Query GetQueryById(string id)
    {
        var query = m_queries.FirstOrDefault(x => x.Id == id) ?? throw new DbKeyNotFoundException(id, typeof(Query));
        return query;
    }

    private async Task AddIterationParameter(List<QueryParameter> queryParameters, IApiClient client)
    {
        var iterations = await client.GetIterationsAsync(GEOMAN_PROJECT, DEV_TEAM);
        queryParameters.Add(new QueryParameter
        {
            Name = ITERATION_PARAMETER,
            DisplayName = "Iteration",
            Description = "Iteration to run the query on",
            Type = Model.Enum.QueryParameterValueType.Select,
            Data = iterations?.Select(x => new { label = $"{x.Name} ({x.Path})", value = x.Id }).ToArray()
        });
    }

    private async Task AddIterationPathParameter(List<QueryParameter> queryParameters, IApiClient client)
    {
        var iterations = await client.GetIterationsAsync(GEOMAN_PROJECT, DEV_TEAM);
        queryParameters.Add(new QueryParameter
        {
            Name = ITERATION_PATH_PARAMETER,
            DisplayName = "Iteration",
            Description = "Iteration to run the query on",
            Type = Model.Enum.QueryParameterValueType.Select,
            Data = iterations?.Select(x => new { label = $"{x.Name} ({x.Path})", value = x.Path }).ToArray()
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

    public async Task<List<QueryParameter>> GetQueryParametersAsync(string queryId)
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
        var query = m_queries.FirstOrDefault(x => x.Id == queryId) ?? throw new KeyNotFoundException();
        using var apiClient = await m_apiClientFactory.GetApiClientAsync();

        object? value;

        // Possible arguments
        Guid iterationId;
        string iterationPath;
        DateTime sprintStart;
        DateTime sprintEnd;

        switch (query.Id)
        {
            case LAST_ITER_WI_TASKS_QUERY:
                iterationId = GetGuidParameterOrThrow(ITERATION_PARAMETER, parameterValues);

                var tasks = await apiClient.GetIterationTasksAsync(new TeamContext(GEOMAN_PROJECT, DEV_TEAM), iterationId);

                value = tasks;
                break;
            case LAST_ITER_WI_TICKETS_QUERY:
                iterationId = GetGuidParameterOrThrow(ITERATION_PARAMETER, parameterValues);

                var userStoriesAndBugs = await apiClient.GetIterationTicketsAsync(new TeamContext(GEOMAN_PROJECT, DEV_TEAM), iterationId);

                value = userStoriesAndBugs;
                break;
            case LAST_ITER_CAP_QUERY:
                iterationId = GetGuidParameterOrThrow(ITERATION_PARAMETER, parameterValues);
                var capacity = await apiClient.GetIterationCapacitiesAsync(new TeamContext(GEOMAN_PROJECT, DEV_TEAM), iterationId);

                value = double.Round(capacity, 2);
                break;
            case LAST_ITER_START_WI_TICKETS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintStart = GetDateTimeParameterOrThrow(SPRINT_START_PARAMETER, parameterValues);

                value = await apiClient.GetNormalTicketsAsOfAsync(GEOMAN_PROJECT, iterationPath, sprintStart);
                break;
            case LAST_ITER_START_WI_TASKS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintStart = GetDateTimeParameterOrThrow(SPRINT_START_PARAMETER, parameterValues);

                value = await apiClient.GetWorkItemsOfIterationAndAsOfAsync(GEOMAN_PROJECT, iterationPath, sprintStart, "Task");
                break;
            case LAST_ITER_END_WI_TICKETS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                value = await apiClient.GetNormalTicketsAsOfAsync(GEOMAN_PROJECT, iterationPath, sprintEnd);
                break;
            case LAST_ITER_END_WI_TASKS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                value = await apiClient.GetWorkItemsOfIterationAndAsOfAsync(GEOMAN_PROJECT, iterationPath, sprintEnd, "Task");
                break;
            case LAST_ITER_WI_TICKET_HAS_BLOCKERS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                value = await apiClient.GetBlockerTicketsAsOfAsync(GEOMAN_PROJECT, iterationPath, sprintEnd);
                break;
            case LAST_ITER_WI_TICKET_HAS_AFTERTHOUGHTTASKS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                value = await apiClient.GetAfterThoughtTicketsAsOfAsync(GEOMAN_PROJECT, iterationPath, sprintEnd);
                break;
            case LAST_ITER_WI_REMOVED_TICKETS_QUERY:
                iterationPath = GetStringParameterOrThrow(ITERATION_PATH_PARAMETER, parameterValues);
                sprintStart = GetDateTimeParameterOrThrow(SPRINT_START_PARAMETER, parameterValues);
                sprintEnd = GetDateTimeParameterOrThrow(SPRINT_END_PARAMETER, parameterValues);

                value = await apiClient.GetRemovedWorkItemsAsync(GEOMAN_PROJECT, iterationPath, sprintEnd, sprintStart, "Task");
                break;
            default:
                throw new KeyNotFoundException($"Query with id {query.Id} not found.");
        }
        var result = new QueryResult
        {
            Type = query.Type,
            Value = value
        };

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
}
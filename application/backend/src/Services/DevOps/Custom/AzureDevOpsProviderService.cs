using System.Security.Cryptography.X509Certificates;
using backend.Model.Analysis;
using backend.Model.Exceptions;
using backend.Services.API;
using Microsoft.TeamFoundation.Core.WebApi.Types;

namespace backend.Services.DevOps.Custom;

public class AzureDevOpsProviderService : IDevOpsProviderService
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly List<Query> _queries;
    private const string LAST_ITER_WI_TICKETS_QUERY = "LAST_ITER_WI_TICKETS_QUERY";
    private const string LAST_ITER_WI_TASKS_QUERY = "LAST_ITER_WI_TASKS_QUERY";
    private const string LAST_ITER_CAP_QUERY = "LAST_ITER_CAP_QUERY";
    private const string GEOMAN_PROJECT = "GeoMan";
    private const string DEV_TEAM = "0_DEV-Team 1";
    public AzureDevOpsProviderService(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory;
        _queries = new List<Query>
        {
            new Query(LAST_ITER_WI_TASKS_QUERY, "GeoMan - Last iteration tasks", Model.Enum.QueryReturnType.ObjectList),
            new Query(LAST_ITER_WI_TICKETS_QUERY, "GeoMan - Last iteration tickets", Model.Enum.QueryReturnType.ObjectList),
            new Query(LAST_ITER_CAP_QUERY, "GeoMan - Last iteration capacity", Model.Enum.QueryReturnType.Number),
        };
    }

    public List<Query> GetQueries() => _queries;
    public Query GetQueryById(string id)
    {
        var query = _queries.FirstOrDefault(x => x.Id == id);
        if (query == null)
            throw new KeyNotFoundException();
        return query;
    }
    public async Task<List<QueryParameter>> GetQueryParametersAsync(string queryId)
    {
        var query = _queries.FirstOrDefault(x => x.Id == queryId);
        if (query == null)
            throw new KeyNotFoundException();

        var queryParameters = new List<QueryParameter>();

        using var client = await _apiClientFactory.GetApiClientAsync();
        switch (query.Id)
        {
            case LAST_ITER_WI_TASKS_QUERY:
            case LAST_ITER_WI_TICKETS_QUERY:
            case LAST_ITER_CAP_QUERY:
                var iterations = await client.GetIterationsAsync(GEOMAN_PROJECT, DEV_TEAM);
                queryParameters.Add(new QueryParameter
                {
                    Name = "iteration",
                    DisplayName = "Iteration",
                    Description = "Iteration to run the query on",
                    Type = Model.Enum.QueryParameterValueType.Select,
                    Data = iterations?.Select(x => new { label = $"{x.Name} ({x.Path})", value = x.Id }).ToArray()
                });
                break;
            default:
                throw new KeyNotFoundException($"Query with id {query.Id} not found.");
        }
        return queryParameters;
    }

    public async Task<QueryResult> ExecuteQueryAsync(string queryId, Dictionary<string, object?> parameterValues)
    {
        var query = _queries.FirstOrDefault(x => x.Id == queryId);
        if (query == null)
            throw new KeyNotFoundException();


        using var apiClient = await _apiClientFactory.GetApiClientAsync();

        object? value;

        switch (query.Id)
        {
            case LAST_ITER_WI_TASKS_QUERY:
                var iterationIdWiQ = GetGuidParameterOrThrow("iteration", parameterValues);

                var tasks = await apiClient.GetIterationTasksAsync(new TeamContext(GEOMAN_PROJECT, DEV_TEAM), iterationIdWiQ);

                value = tasks;
                break;
            case LAST_ITER_WI_TICKETS_QUERY:
                var iterationIdWiQT = GetGuidParameterOrThrow("iteration", parameterValues);

                var userStoriesAndBugs = await apiClient.GetIterationTicketsAsync(new TeamContext(GEOMAN_PROJECT, DEV_TEAM), iterationIdWiQT);

                value = userStoriesAndBugs;
                break;
            case LAST_ITER_CAP_QUERY:
                var iterationIdCap = GetGuidParameterOrThrow("iteration", parameterValues);
                var capacity = await apiClient.GetIterationCapacities(GEOMAN_PROJECT, iterationIdCap);

                value = capacity;
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

    private Guid GetGuidParameterOrThrow(string paramName, Dictionary<string, object?> parameterValues)
    {
        if (!parameterValues.TryGetValue(paramName, out var parameter))
            throw new QueryExecuteException();
        if (parameter == null || !Guid.TryParse(parameter.ToString(), out var iterationId))
            throw new QueryExecuteException();

        return iterationId;
    }
}
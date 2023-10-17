
using backend.Model.Analysis;

namespace backend.Services.DevOps;

public interface IDevOpsProviderService
{
    Task<List<Query>> GetQueries();
    Query GetQueryById(string id);
    Task<List<QueryParameter>> GetQueryRuntimeParametersAsync(string queryId);
    Task<QueryResult> ExecuteQueryAsync(string queryId, Dictionary<string, object?> runtimeParameterValues);

    /// <summary>
    /// Indicates if the service has a valid configuration
    /// </summary>
    /// <returns></returns>
    bool HasValidConfiguration();
}
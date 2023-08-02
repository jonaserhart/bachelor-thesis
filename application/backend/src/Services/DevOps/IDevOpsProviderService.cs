
using backend.Model.Analysis;

namespace backend.Services.DevOps;

public interface IDevOpsProviderService
{
    List<Query> GetQueries();
    Query GetQueryById(string id);
    Task<List<QueryParameter>> GetQueryParametersAsync(string queryId);
    Task<QueryResult> ExecuteQueryAsync(string queryId, Dictionary<string, object?> parameterValues);
}
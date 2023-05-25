using backend.Model.Analysis;
using backend.Model.Rest;

namespace backend.Services.DevOps;

public interface IQueryService
{
    Task<Query> GetQueryByIdAsync(Guid queryId);
    Task<IEnumerable<QueryResponse>> GetQueriesAsync(string projectId);
    Task<Query> CreateQueryFromDevOps(Guid modelId, Guid queryId);
    Task<Query> GetQueryWithClauses(Guid queryId);
}
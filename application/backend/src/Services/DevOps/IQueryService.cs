using backend.Model.Analysis;
using backend.Model.Analysis.Queries;
using backend.Model.Rest;

namespace backend.Services.DevOps;

public interface IQueryService
{
    IEnumerable<BaseQuerySchema> GetBaseQuerySchemas();
    Task<IEnumerable<QueryParameter>> GetQueryCreateParametersAsync(Guid schemaId);
    Task<Model.Analysis.Queries.Query> CreateQueryAsync(Guid schemaId, List<QueryParameterValue> queryParameterValue);
    Task<IEnumerable<QueryParameter>> GetQueryRuntimeParametersAsync(Guid schemaId, Guid queryId);
    Task<object?> ExecuteQueryAsync(Guid schemaId, Guid queryId, List<QueryParameterValue> runtimeParams);
    Task<Model.Analysis.Query> GetQueryByIdAsync(Guid queryId);
    Task<IEnumerable<QueryResponse>> GetQueriesAsync(string projectId);
    Task<Model.Analysis.Query> CreateQueryFromDevOps(Guid modelId, Guid queryId);
    Task<Model.Analysis.Query> GetQueryWithClausesAsync(Guid queryId);
    Task<QueryChange> UpdateQueryAsync(QueryChange queryChange);
}
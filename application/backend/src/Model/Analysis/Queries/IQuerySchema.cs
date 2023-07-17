using backend.Model.Enum;

namespace backend.Model.Analysis.Queries;

public interface IQuerySchema
{
    Guid Id { get; }
    string Description { get; }
    QueryReturnType ReturnType { get; }
    Task<List<QueryParameter>> GetCreateQueryParametersAsync();
    Task<Query> CreateQueryAsync(List<QueryParameterValue> createQueryParametersAndValues);
    Task<List<QueryParameter>> GetRuntimeParametersAsync(Query query);
    Task<object> ExecuteQueryAsync(Query query, List<QueryParameterValue> runtimeParameters);
}

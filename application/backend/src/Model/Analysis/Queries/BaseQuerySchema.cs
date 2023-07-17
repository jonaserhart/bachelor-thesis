using backend.Model.Enum;

namespace backend.Model.Analysis.Queries;

public abstract class BaseQuerySchema
{
    protected BaseQuerySchema(Guid id, string description, QueryReturnType returnType)
    {
        Id = id;
        Description = description;
        ReturnType = returnType;
    }

    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public QueryReturnType ReturnType { get; set; }

    public abstract Task<List<QueryParameter>> GetCreateQueryParametersAsync();
    public abstract Task<Query> CreateQueryAsync(List<QueryParameterValue> createQueryParametersAndValues);

    public abstract Task<List<QueryParameter>> GetRuntimeParametersAsync(Query query);
    public abstract Task<object> ExecuteQueryAsync(Query query, List<QueryParameterValue> runtimeParameters);


    public override bool Equals(object? obj) => obj is BaseQuerySchema schema && Id.Equals(schema.Id);
    public override int GetHashCode() => Id.GetHashCode();
}
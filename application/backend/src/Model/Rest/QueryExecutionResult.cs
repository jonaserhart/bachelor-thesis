using backend.Model.Enum;

namespace backend.Model.Rest;

public class QueryExecutionResult
{
    public object? Result { get; set; }
    public QueryReturnType ReturnType { get; set; }
}
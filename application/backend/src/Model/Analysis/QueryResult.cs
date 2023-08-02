using backend.Model.Enum;

namespace backend.Model.Analysis;

public class QueryResult
{
    public object? Value { get; set; }
    public QueryReturnType Type { get; set; }
}
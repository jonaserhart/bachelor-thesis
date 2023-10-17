using backend.Model.Enum;

namespace backend.Model.Analysis;

public class QueryResult
{
    public object? Value { get; set; }
    public QueryReturnType Type { get; set; }
    public List<QueryParameterValue> ParameterValues { get; set; } = new List<QueryParameterValue>();
}
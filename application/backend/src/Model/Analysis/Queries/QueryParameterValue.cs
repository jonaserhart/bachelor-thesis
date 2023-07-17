using backend.Model.Enum;

namespace backend.Model.Analysis.Queries;

public class QueryParameterValue
{
    public string? Key { get; set; }
    public CreateQueryParameterType Type { get; set; }
    public object? Value { get; set; }
}
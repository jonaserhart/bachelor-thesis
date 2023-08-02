using backend.Model.Enum;

namespace backend.Model.Analysis;

public class QueryParameter
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public QueryParameterValueType Type { get; set; }
    public object? Data { get; set; }
}
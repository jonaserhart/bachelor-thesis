using backend.Model.Enum;

namespace backend.Model.Analysis.Queries;

public class QueryParameter
{
    public QueryParameter(string key, CreateQueryParameterType type)
    {
        Key = key;
        Type = type;
    }

    public string Key { get; set; }
    public CreateQueryParameterType Type { get; set; }
    public object? Data { get; set; }
}
using backend.Model.Analysis;

namespace backend.Model.Rest;

public class QueryChange
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
namespace backend.Model.Analysis.Queries;

public class Query
{
    public Guid Id { get; set; }
    public string QueryData { get; set; } = string.Empty;
    public List<string> RuntimeParameters { get; set; } = new List<string>();
}
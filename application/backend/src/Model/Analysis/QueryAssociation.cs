
namespace backend.Model.Analysis;

public class QueryAssociation
{
    public AnalysisModel? Model { get; set; }
    public Guid ModelId { get; set; }
    public string? QueryId { get; set; }
    public List<QueryParameterValue> ParametersAndValues { get; set; } = new List<QueryParameterValue>();
}
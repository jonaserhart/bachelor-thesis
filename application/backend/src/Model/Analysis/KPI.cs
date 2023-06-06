using backend.Model.Analysis.Expressions;
using Newtonsoft.Json;

namespace backend.Model.Analysis;

public class KPI
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Expression? Expression { get; set; }

    [JsonIgnore]
    public AnalysisModel? AnalysisModel { get; set; }

    [JsonIgnore]
    public Guid? AnalysisModelId { get; set; }
}
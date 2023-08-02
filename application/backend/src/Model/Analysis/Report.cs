using Newtonsoft.Json;

namespace backend.Model.Analysis;

public class Report
{
    public Guid Id { get; set; }
    public Guid? AnalysisModelId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public long Created { get; set; }
    [JsonIgnore]
    public AnalysisModel? AnalysisModel { get; set; }
    public Dictionary<string, QueryResult> QueryResults { get; set; } = new Dictionary<string, QueryResult>();

    [JsonProperty("kpisAndValues")]
    public Dictionary<Guid, object?> KPIsAndValues { get; set; } = new Dictionary<Guid, object?>();
}

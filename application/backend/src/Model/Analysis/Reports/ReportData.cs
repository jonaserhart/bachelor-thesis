
using Newtonsoft.Json;

namespace backend.Model.Analysis.Reports;

public class ReportData
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Report? Report { get; set; }
    public Guid ReportId { get; set; }
    public Dictionary<string, QueryResult> QueryResults { get; set; } = new Dictionary<string, QueryResult>();

    [JsonProperty("kpisAndValues")]
    public Dictionary<Guid, object?> KPIsAndValues { get; set; } = new Dictionary<Guid, object?>();
}
using Newtonsoft.Json;

namespace backend.Model.Analysis.Graphical;

public class GraphicalItemDataSources
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    [JsonIgnore]
    public GraphicalReportItem? GraphicalReportItem { get; set; }

    [JsonProperty("kpis")]
    public List<Guid> KPIs { get; set; } = new List<Guid>();
}

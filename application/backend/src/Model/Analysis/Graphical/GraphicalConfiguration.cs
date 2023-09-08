using Newtonsoft.Json;

namespace backend.Model.Analysis.Graphical;

public class GraphicalConfiguration
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public AnalysisModel? Model { get; set; }
    public List<GraphicalReportItem> Items { get; set; } = new List<GraphicalReportItem>();
}

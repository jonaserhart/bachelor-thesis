using Newtonsoft.Json;

namespace backend.Model.Analysis.Graphical;

public class GraphicalReportItemProperties
{
    public Guid Id { get; set; }
    public List<string> ListFields { get; set; } = new List<string>();

    [JsonIgnore]
    public GraphicalReportItem? Item { get; set; }
    public Guid ItemId { get; set; }
}
using backend.Model.Enum;
using Newtonsoft.Json;

namespace backend.Model.Analysis.Graphical;

public class GraphicalReportItem
{
    public Guid Id { get; set; }
    public GraphicalReportItemType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public GraphicalReportItemProperties? Properties { get; set; }

    [JsonIgnore]
    public GraphicalConfiguration? Configuration { get; set; }
    public GraphicalReportItemLayout? Layout { get; set; }
    public GraphicalItemDataSources? DataSources { get; set; }

}

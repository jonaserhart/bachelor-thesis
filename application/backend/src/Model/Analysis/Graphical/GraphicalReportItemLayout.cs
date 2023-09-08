using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace backend.Model.Analysis.Graphical;

public class GraphicalReportItemLayout
{
    [Key]
    public Guid Id { get; set; }

    public Guid I { get; set; }
    [JsonIgnore]
    public GraphicalReportItem? GraphicalReportItem { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
    public int? MaxW { get; set; }
    public int? MaxH { get; set; }
    public int? MinW { get; set; }
    public int? MinH { get; set; }
}

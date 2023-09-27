using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Model.Analysis.Graphical;
using backend.Model.Analysis.Reports;
using backend.Model.Analysis.KPIs;
using backend.Model.Users;
using Newtonsoft.Json;

namespace backend.Model.Analysis;

public class AnalysisModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<UserModel> ModelUsers { get; set; } = new List<UserModel>();
    [JsonProperty("kpis")]
    public List<KPI> KPIs { get; set; } = new List<KPI>();
    [JsonProperty("kpiFolders")]
    public List<KPIFolder> KPIFolders { get; set; } = new List<KPIFolder>();
    public List<Report> Reports { get; set; } = new List<Report>();
    public List<GraphicalConfiguration> Graphical { get; set; } = new List<GraphicalConfiguration>();

}
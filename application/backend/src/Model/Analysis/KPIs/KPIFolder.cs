using Newtonsoft.Json;

namespace backend.Model.Analysis.KPIs;

public class KPIFolder
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Guid? ParentFolderId { get; set; }

    [JsonIgnore]
    public KPIFolder? ParentFolder { get; set; }

    public Guid? ModelId { get; set; }
    [JsonIgnore]
    public AnalysisModel? AnalysisModel { get; set; }

    [JsonProperty("kpis")]
    public List<KPI> KPIs { get; set; } = new List<KPI>();
    public List<KPIFolder> SubFolders { get; set; } = new List<KPIFolder>();
}


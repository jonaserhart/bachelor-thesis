using Newtonsoft.Json;

namespace backend.Model.Analysis.Reports;

public class Report
{
    public Guid Id { get; set; }
    public Guid? AnalysisModelId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public long Created { get; set; }
    [JsonIgnore]
    public AnalysisModel? AnalysisModel { get; set; }
    public ReportData? ReportData { get; set; }
}

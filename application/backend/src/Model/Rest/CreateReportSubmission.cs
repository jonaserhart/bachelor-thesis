using backend.Model.Analysis;

namespace backend.Model.Rest;

public class CreateReportSubmission
{
    public Dictionary<string, object?> QueryParameterValues { get; set; } = new Dictionary<string, object?>();
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
namespace backend.Model.Rest;

public class AnalysisModelRequest
{
    public string Name { get; set; } = string.Empty;
    public ProjectRequest? Project { get; set; }
}
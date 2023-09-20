namespace backend.Model.Config;

public class DevOpsConfig
{
    public string ServerUrl { get; set; } = string.Empty;
    public List<TokenSource> TokenSources { get; set; } = new List<TokenSource>();
}
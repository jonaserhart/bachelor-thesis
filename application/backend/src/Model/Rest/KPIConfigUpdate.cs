
namespace backend.Model.Rest;

public class KPIConfigUpdate
{
    public bool ShowInReport { get; set; }
    public string AcceptableValues { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}
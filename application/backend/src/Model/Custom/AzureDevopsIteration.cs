namespace backend.Model.Custom;

public class AzureDevopsIteration
{
    public Guid Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
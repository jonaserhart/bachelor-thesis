namespace backend.Model.Rest;

public class FieldInfoSubmission
{
    public Guid? Id { get; set; }
    public string ReferenceName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
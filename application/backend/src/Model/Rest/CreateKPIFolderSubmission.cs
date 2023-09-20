namespace backend.Model.Rest;

public class CreateKPIFolderSubmission
{
    public string Name { get; set; } = string.Empty;
    public Guid? FolderId { get; set; }
}
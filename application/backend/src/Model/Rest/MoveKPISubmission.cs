namespace backend.Model.Rest;

public class MoveKPISubmission
{
    public Guid Id { get; set; }
    public Guid? MoveToFolder { get; set; }
    public Guid? MoveToModel { get; set; }
}

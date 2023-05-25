namespace backend.Model.Rest;

public class ConditionSubmission
{
    public Guid? Id { get; set; }
    public string AndOr { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
namespace backend.Model.Rest;

public class OAuthCallbackModel
{
    public string Code { get; set; } = string.Empty;
    public Guid State { get; set; }
}
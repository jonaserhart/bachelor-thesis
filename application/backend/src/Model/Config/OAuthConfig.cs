namespace backend.Model.Config;

public class OAuthConfig
{
    public string AuthorizationUri { get; set; } = string.Empty;
    public string TokenUri { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
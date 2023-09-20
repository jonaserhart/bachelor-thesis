using backend.Model.Rest;

namespace backend.Model.Config;

public class AuthenticationConfig
{
    public List<AuthMethod> AvailableMethods { get; set; } = new List<AuthMethod>();
    public OAuthConfig? OAuth { get; set; }
    public string CookieDomain { get; set; } = string.Empty;
}
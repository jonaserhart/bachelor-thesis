using System.Runtime.Serialization;

namespace backend.Model;

[DataContract]
public class TokenModel
{    
    [DataMember(Name = "access_token")]
    public String AccessToken { get; set; } = string.Empty;

    [DataMember(Name = "token_type")]
    public String TokenType { get; set; } = string.Empty;

    [DataMember(Name = "refresh_token")]
    public String RefreshToken { get; set; } = string.Empty;

    [DataMember(Name = "expires_in")]
    public int ExpiresIn { get; set; }

    public bool IsPending { get; set; }
}
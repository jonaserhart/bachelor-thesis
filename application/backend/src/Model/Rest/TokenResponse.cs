using Newtonsoft.Json;

namespace backend.Model.Rest;

public class TokenResponse
{
    public string JWT { get; set; } = string.Empty;

    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;

    public long Expires { get; set; }

    public static TokenResponse From(TokenModel tokenModel)
    {
        return new TokenResponse
        {
            Expires = tokenModel.ExpiresIn,
            JWT = tokenModel.AccessToken,
            RefreshToken = tokenModel.RefreshToken
        };
    }
}
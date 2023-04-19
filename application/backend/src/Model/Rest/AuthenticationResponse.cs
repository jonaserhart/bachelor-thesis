namespace backend.Model.Rest;

public class AuthenticationResponse
{
    public UserResponse? User { get; set; }
    public TokenResponse? Token { get; set; }
}
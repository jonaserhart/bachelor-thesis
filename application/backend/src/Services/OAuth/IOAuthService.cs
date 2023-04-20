using backend.Model;
using backend.Model.Rest;

namespace backend.Services.OAuth;

public interface IOAuthService
{
    string GetNewAuthorizeUrl();
    Task<AuthenticationResponse> HandleOAuthCallbackAsync(OAuthCallbackModel submission);
    Task<AuthenticationResponse> RefreshTokenAsync(string? token);
}
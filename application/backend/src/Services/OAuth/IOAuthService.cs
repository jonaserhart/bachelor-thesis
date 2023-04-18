using backend.Model;
using backend.Model.Rest;

namespace backend.Services.OAuth;

public interface IOAuthService
{
    string GetNewAuthorizeUrl();
    Task<TokenModel> HandleOAuthCallback(OAuthCallbackModel submission);
}
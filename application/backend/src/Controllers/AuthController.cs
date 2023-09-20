using Microsoft.AspNetCore.Mvc;
using backend.Model.Rest;
using backend.Services.OAuth;
using backend.Model.Config;
using Microsoft.Extensions.Options;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly IOAuthService m_authService;
    private readonly AuthenticationConfig m_authenticationConfig;
    public AuthController(IOAuthService oAuthService, IOptions<AuthenticationConfig> options)
    {
        m_authService = oAuthService;
        m_authenticationConfig = options.Value;
    }

    [HttpGet("oauth-authorize")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public ActionResult<string> Authorize()
    {
        var url = m_authService.GetNewAuthorizeUrl();
        return Ok(url);
    }

    [HttpGet("methods")]
    [ProducesResponseType(typeof(AuthMethod[]), 200)]
    public ActionResult<AuthMethod[]> GetAuthMethods() => Ok(m_authenticationConfig.AvailableMethods);

    [HttpPost("callback")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public async Task<ActionResult> Callback([FromBody] OAuthCallbackModel submission)
    {
        var response = await m_authService.HandleOAuthCallbackAsync(submission);
        SetTokenCookie(response.Token?.RefreshToken);
        return Ok(response);
    }

    [HttpGet("refresh-token")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await m_authService.RefreshTokenAsync(refreshToken);
        SetTokenCookie(response.Token?.RefreshToken);
        return Ok(response);
    }

    private void SetTokenCookie(string? token)
    {
        if (token == null)
            return;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7),
        };

        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

}
using Microsoft.AspNetCore.Mvc;
using backend.Model.Rest;
using backend.Services.OAuth;
using backend.Model;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class OAuthController : Controller
{
    private readonly IOAuthService m_oauthService;
    public OAuthController(IOAuthService oAuthService) => m_oauthService = oAuthService;

    [HttpGet("authorize")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public ActionResult<string> Authorize()
    {
        var url = m_oauthService.GetNewAuthorizeUrl();
        return Ok(url);
    }

    [HttpPost("callback")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public async Task<ActionResult> Callback([FromBody] OAuthCallbackModel submission)
    {
        var response = await m_oauthService.HandleOAuthCallbackAsync(submission);
        SetTokenCookie(response.Token?.RefreshToken);
        return Ok(response);
    }

    [HttpGet("refresh-token")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await m_oauthService.RefreshTokenAsync(refreshToken);
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
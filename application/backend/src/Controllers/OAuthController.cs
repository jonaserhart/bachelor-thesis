using Microsoft.AspNetCore.Mvc;
using backend.Model.Rest;
using backend.Services.OAuth;
using backend.Model;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class OAuthController : Controller
{
    private readonly ILogger<OAuthController> _logger;
    private readonly IOAuthService _oauthService;
    public OAuthController(ILogger<OAuthController> logger, IOAuthService oAuthService)
    {
        _logger = logger;
        _oauthService = oAuthService;
    }

    [HttpGet("authorize")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public ActionResult<string> Authorize()
    {
        var url = _oauthService.GetNewAuthorizeUrl();
        return Ok(url);
    }

    [HttpPost("callback")]
    [ProducesResponseType(typeof(TokenModel), 200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public async Task<ActionResult> Callback([FromBody] OAuthCallbackModel submission)
    {
        var token = await _oauthService.HandleOAuthCallback(submission);
        SetTokenCookie(token.RefreshToken);
        return Ok(token);
    }

    private void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7),
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

}
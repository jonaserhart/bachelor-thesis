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
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(typeof(ApiError), 400)]
    public async Task<ActionResult> Callback([FromBody] OAuthCallbackModel submission)
    {
        var response = await _oauthService.HandleOAuthCallbackAsync(submission);
        SetTokenCookie(response.Token?.RefreshToken);
        return Ok(response);
    }


    [HttpGet("refresh-token")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _oauthService.RefreshTokenAsync(refreshToken);
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
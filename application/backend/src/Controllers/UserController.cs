using backend.Model.Rest;
using backend.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserService m_userService;

    public UserController(IUserService userService) => m_userService = userService;

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    [ProducesResponseType(typeof(ApiError), 401)]
    public async Task<ActionResult<UserResponse>> GetSelfAsync()
    {
        var user = await m_userService.GetSelfAsync();
        return Ok(UserResponse.From(user));
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkItemsController : Controller
{
    private readonly ILogger<WorkItemsController> _logger;

    public WorkItemsController(ILogger<WorkItemsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(bool), 200)]
    [Authorize]
    public bool Health()
    {
        _logger.LogInformation($"Health requested by user id {HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value} IsAuthenticated: {HttpContext.User.Identity?.IsAuthenticated ?? false} ({string.Join(',', HttpContext.User.Claims.Select(x => x.Type))})");
        return true;
    }
}
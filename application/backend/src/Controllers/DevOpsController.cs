using System.Security.Claims;
using backend.Model.Analysis;
using backend.Model.Rest;
using backend.Services.DevOps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DevOpsController : Controller
{
    private readonly ILogger<DevOpsController> _logger;
    private readonly IAnalysisModelService _analysisModelService;

    public DevOpsController(ILogger<DevOpsController> logger, IAnalysisModelService analysisModelService)
    {
        _logger = logger;
        _analysisModelService = analysisModelService;
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(bool), 200)]
    [Authorize]
    public bool Health()
    {
        _logger.LogInformation($"Health requested by user id {HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value} IsAuthenticated: {HttpContext.User.Identity?.IsAuthenticated ?? false} ({string.Join(',', HttpContext.User.Claims.Select(x => x.Type))})");
        return true;
    }

    [HttpGet("mymodels")]
    [ProducesResponseType(typeof(AnalysisModel[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AnalysisModel>>> GetMyModels()
    {
        var models = await _analysisModelService.GetMyModelsAsync();
        return Ok(models);
    }

    [HttpGet("projects")]
    [ProducesResponseType(typeof(Project[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Project>>> GetMyProjects()
    {
        var projects = await _analysisModelService.GetProjectsAsync();
        return Ok(projects);
    }

    [HttpGet("teams/{projectId}")]
    [ProducesResponseType(typeof(Team[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Project>>> GetTeams(string projectId)
    {
        var teams = await _analysisModelService.GetTeamsAsync(projectId);
        return Ok(teams);
    }

    [HttpGet("fields/{projectId}")]
    [ProducesResponseType(typeof(FieldInfo[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Project>>> GetFields(string projectId)
    {
        var fields = await _analysisModelService.GetFieldInfosAsync(projectId);
        return Ok(fields);
    }

    [HttpGet("getmodel/{id}")]
    [ProducesResponseType(typeof(Project[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Project>>> GetModelById(Guid id)
    {
        var model = await _analysisModelService.GetByIdAsync(id);
        return Ok(model);
    }

    [HttpPost("createmodel")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    [Authorize]
    public async Task<ActionResult<AnalysisModel>> CreateModel(AnalysisModelRequest request)
    {
        var model = await _analysisModelService.CreateAsync(request);
        return Ok(model);
    }

    [HttpPut("updatemodel/{id}")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    [Authorize]
    public async Task<ActionResult<AnalysisModel>> UpdateModel(Guid id, [FromBody]AnalysisModelUpdate request)
    {
        var model = await _analysisModelService.UpdateAsync(id, request);
        return Ok(model);
    }
}
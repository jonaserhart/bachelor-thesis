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
    private readonly IQueryService _queryService;

    public DevOpsController(ILogger<DevOpsController> logger, IAnalysisModelService analysisModelService, IQueryService queryService)
    {
        _logger = logger;
        _analysisModelService = analysisModelService;
        _queryService = queryService;
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
    public async Task<ActionResult<IEnumerable<Team>>> GetTeams(string projectId)
    {
        var teams = await _analysisModelService.GetTeamsAsync(projectId);
        return Ok(teams);
    }

    [HttpGet("fields/{projectId}")]
    [ProducesResponseType(typeof(FieldInfo[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FieldInfo>>> GetFields(string projectId)
    {
        var fields = await _analysisModelService.GetFieldInfosAsync(projectId);
        return Ok(fields);
    }

    [HttpGet("queries/{projectId}")]
    [ProducesResponseType(typeof(QueryResponse[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<QueryResponse>>> GetQueries(string projectId)
    {
        var queries = await _queryService.GetQueriesAsync(projectId);
        return Ok(queries);
    }

    [HttpGet("model/{id}/details")]
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

    [HttpPut("model/{id}/update")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    [Authorize]
    public async Task<ActionResult<AnalysisModel>> UpdateModel(Guid id, [FromBody] AnalysisModelUpdate request)
    {
        var model = await _analysisModelService.UpdateAsync(id, request);
        return Ok(model);
    }

    [HttpPost("createqueryfrom")]
    [ProducesResponseType(typeof(Query), 200)]
    [Authorize]
    public async Task<ActionResult<Query>> CreateQueryFrom(Guid modelId, Guid queryId)
    {
        var query = await _queryService.CreateQueryFromDevOps(modelId, queryId);
        return Ok(query);
    }

    [HttpGet("query/{queryId}")]
    [ProducesResponseType(typeof(Query), 200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    [Authorize]
    public async Task<ActionResult<Query>> GetQuery(Guid queryId)
    {
        var query = await _queryService.GetQueryWithClauses(queryId);
        return query;
    }

    [HttpPut("query")]
    [ProducesResponseType(typeof(QueryChange), 200)]
    [ProducesResponseType(typeof(ApiError), 404)]
    [ProducesResponseType(typeof(ApiError), 400)]
    [Authorize]
    public async Task<ActionResult<QueryChange>> UpdateQuery([FromBody] QueryChange queryChange)
    {
        var changes = await _queryService.UpdateQueryAsync(queryChange);
        return changes;
    }
}
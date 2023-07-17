using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Analysis.WorkItems;
using backend.Model.Rest;
using backend.Services.DevOps;
using backend.Services.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AnalysisController : Controller
{
    private readonly ILogger<AnalysisController> _logger;
    private readonly IAnalysisModelService _analysisModelService;
    private readonly IQueryService _queryService;
    private readonly IKPIService _kpiService;

    public AnalysisController(ILogger<AnalysisController> logger,
                            IAnalysisModelService analysisModelService,
                            IQueryService queryService,
                            IKPIService kpiService)
    {
        _logger = logger;
        _analysisModelService = analysisModelService;
        _queryService = queryService;
        _kpiService = kpiService;
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
        var query = await _queryService.GetQueryWithClausesAsync(queryId);
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

    [HttpPost("workItems")]
    [ProducesResponseType(typeof(Workitem[]), 200)]
    [Authorize]
    public async Task<IEnumerable<Workitem>> GetWorkitemsAsync(string project, Guid queryid, [FromBody] Iteration iteration)
    {
        return await _analysisModelService.GetWorkitemsAsync(project, iteration, queryid);
    }

    [HttpPost("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<KPI> CreateKPI(Guid modelId)
    {
        var kpi = await _kpiService.CreateNewKPIAsync(modelId);
        return kpi;
    }

    [HttpGet("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<KPI> GetKPI(Guid id)
    {
        var kpi = await _kpiService.GetByIdAsync(id);
        return kpi;
    }

    [HttpPut("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<KPI> UpdateKPI([FromBody] KPIUpdate update)
    {
        var kpi = await _kpiService.UpdateKPIAsync(update);
        return kpi;
    }

    [HttpDelete("kpi")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task UpdateKPI(Guid id)
    {
        await _kpiService.DeleteKPIAsync(id);
    }

    [HttpPost("kpi/expression")]
    [ProducesResponseType(typeof(Expression), 200)]
    [Authorize]
    public async Task<Expression> AddExpression([FromQuery] Guid kpiId, [FromBody] ExpressionSubmission expression)
    {
        if (expression.Expression == null)
        {
            throw new ArgumentException("property 'expression' has to be provided!");
        }
        return await _kpiService.SaveExpressionAsync(kpiId, expression.Expression);
    }

    [HttpPut("kpi/expression")]
    [ProducesResponseType(typeof(Expression), 200)]
    [Authorize]
    public async Task<Expression> UpdateExpression([FromQuery] Guid kpiId, [FromBody] ExpressionSubmission expression)
    {
        if (expression.Expression == null)
        {
            throw new ArgumentException("property 'expression' has to be provided!");
        }
        return await _kpiService.UpdateExpressionAsync(kpiId, expression.Expression);
    }

}
using System.Runtime.Serialization;
using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
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
    private readonly IKPIService _kpiService;
    private readonly IDevOpsProviderService _devOpsProviderService;

    public AnalysisController(ILogger<AnalysisController> logger,
                            IAnalysisModelService analysisModelService,
                            IKPIService kpiService,
                            IDevOpsProviderService devOpsProviderService)
    {
        _logger = logger;
        _analysisModelService = analysisModelService;
        _kpiService = kpiService;
        _devOpsProviderService = devOpsProviderService;

    }

    [HttpGet("mymodels")]
    [ProducesResponseType(typeof(AnalysisModel[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AnalysisModel>>> GetMyModels()
    {
        var models = await _analysisModelService.GetMyModelsAsync();
        return Ok(models);
    }

    [HttpGet("model/{id}/details")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    [Authorize]
    public async Task<ActionResult<AnalysisModel>> GetModelById(Guid id)
    {
        var model = await _analysisModelService.GetByIdAsync(id);
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

    [HttpPost("model/createreport")]
    [ProducesResponseType(typeof(Report), 200)]
    [Authorize]
    public async Task<ActionResult<Report>> CreateReport([FromBody] CreateReportSubmission submission)
    {
        var report = await _analysisModelService.CreateReportAsync(submission);
        return Ok(report);
    }

    [HttpDelete("model/deletereport")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<ActionResult> DeleteReport([FromQuery] Guid reportId)
    {
        await _analysisModelService.DeleteReportAsync(reportId);
        return Ok();
    }

    [HttpPost("createmodel")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    [Authorize]
    public async Task<ActionResult<AnalysisModel>> CreateModel(AnalysisModelRequest request)
    {
        var model = await _analysisModelService.CreateAsync(request);
        return Ok(model);
    }

    [HttpGet("customqueries")]
    [ProducesResponseType(typeof(Query[]), 200)]
    [Authorize]
    public ActionResult<IEnumerable<Query>> GetCustomQueries()
    {
        var queries = _devOpsProviderService.GetQueries();
        return Ok(queries);
    }

    [HttpGet("requiredQueries")]
    [ProducesResponseType(typeof(string[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<string>>> GetRequiredQueries([FromQuery] Guid modelId)
    {
        var queries = await _analysisModelService.GetRequiredQueriesAsync(modelId);
        return Ok(queries);
    }

    [HttpGet("customquery")]
    [ProducesResponseType(typeof(Query), 200)]
    [Authorize]
    public ActionResult<IEnumerable<Query>> GetCustomQueries([FromQuery] string queryId)
    {
        var query = _devOpsProviderService.GetQueryById(queryId);
        return Ok(query);
    }

    [HttpGet("queryparameters")]
    [ProducesResponseType(typeof(QueryParameter[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Query>>> GetCustomQueryParameters([FromQuery] string queryId)
    {
        var parameters = await _devOpsProviderService.GetQueryParametersAsync(queryId);
        return Ok(parameters);
    }

    [HttpPost("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<ActionResult<KPI>> CreateKPI(Guid modelId)
    {
        var kpi = await _kpiService.CreateNewKPIAsync(modelId);
        return Ok(kpi);
    }

    [HttpGet("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<ActionResult<KPI>> GetKPI(Guid id)
    {
        var kpi = await _kpiService.GetByIdAsync(id);
        return Ok(kpi);
    }

    [HttpPut("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<ActionResult<KPI>> UpdateKPI([FromBody] KPIUpdate update)
    {
        var kpi = await _kpiService.UpdateKPIAsync(update);
        return Ok(kpi);
    }

    [HttpPut("kpi/config")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<ActionResult<KPIConfigUpdate>> UpdateKPIConfig([FromQuery] Guid id, [FromBody] KPIConfigUpdate update)
    {
        var kpi = await _kpiService.UpdateKPIConfigAsync(id, update);
        return Ok(kpi);
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
    public async Task<ActionResult<Expression>> AddExpression([FromQuery] Guid kpiId, [FromBody] ExpressionSubmission expression)
    {
        if (expression.Expression == null)
        {
            throw new ArgumentException("property 'expression' has to be provided!");
        }
        var saved = await _kpiService.SaveExpressionAsync(kpiId, expression.Expression);
        return Ok(saved);
    }

    [HttpPut("kpi/expression")]
    [ProducesResponseType(typeof(Expression), 200)]
    [Authorize]
    public async Task<ActionResult<Expression>> UpdateExpression([FromQuery] Guid kpiId, [FromBody] ExpressionSubmission expression)
    {
        if (expression.Expression == null)
        {
            throw new ArgumentException("property 'expression' has to be provided!");
        }
        var updated = await _kpiService.UpdateExpressionAsync(kpiId, expression.Expression);
        return Ok(updated);
    }

}
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Runtime.Serialization;
using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Analysis.Graphical;
using backend.Model.Analysis.KPIs;
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
    private readonly IAnalysisModelService m_analysisModelService;
    private readonly IKPIService m_kpiService;
    private readonly IDevOpsProviderService m_devOpsProviderService;

    public AnalysisController(
                            IAnalysisModelService analysisModelService,
                            IKPIService kpiService,
                            IDevOpsProviderService devOpsProviderService)
    {
        m_analysisModelService = analysisModelService;
        m_kpiService = kpiService;
        m_devOpsProviderService = devOpsProviderService;

    }

    [HttpGet("mymodels")]
    [ProducesResponseType(typeof(AnalysisModel[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AnalysisModel>>> GetMyModels()
    {
        var models = await m_analysisModelService.GetMyModelsAsync();
        return Ok(models);
    }

    [HttpGet("model/{id}/details")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    [Authorize]
    public async Task<ActionResult<AnalysisModel>> GetModelById(Guid id)
    {
        var model = await m_analysisModelService.GetByIdAsync(id);
        return Ok(model);
    }

    [HttpPut("model/{id}/update")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    [Authorize]
    public async Task<ActionResult<AnalysisModel>> UpdateModel(Guid id, [FromBody] AnalysisModelUpdate request)
    {
        var model = await m_analysisModelService.UpdateAsync(id, request);
        return Ok(model);
    }

    [HttpGet("model/graphicalconfig")]
    [ProducesResponseType(typeof(GraphicalConfiguration), 200)]
    [Authorize]
    public async Task<ActionResult<GraphicalConfiguration>> GetGraphicalConfig([FromQuery] Guid graphicalId)
    {
        var config = await m_analysisModelService.GetGraphicalConfigAsync(graphicalId);
        return Ok(config);
    }

    [HttpPost("model/graphicalconfig")]
    [ProducesResponseType(typeof(GraphicalConfiguration), 200)]
    [Authorize]
    public async Task<ActionResult<GraphicalConfiguration>> CreateNewGraphicalConfig([FromQuery] Guid modelId)
    {
        var config = await m_analysisModelService.CreateGraphicalConfigAsync(modelId);
        return Ok(config);
    }

    [HttpPut("model/graphicalconfig")]
    [ProducesResponseType(typeof(GraphicalConfiguration), 200)]
    [Authorize]
    public async Task<ActionResult<GraphicalConfiguration>> UpdateGraphicalConfig([FromQuery] Guid id, [FromBody] GraphicalConfigurationUpdate submission)
    {
        var config = await m_analysisModelService.UpdateConfigAsync(id, submission);
        return Ok(config);
    }

    [HttpDelete("model/graphicalconfig")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<ActionResult> DeleteGraphicalConfig([FromQuery] Guid id)
    {
        await m_analysisModelService.DeleteConfigAsync(id);
        return Ok();
    }

    [HttpPost("model/graphicalconfig/item")]
    [ProducesResponseType(typeof(GraphicalReportItem), 200)]
    [Authorize]
    public async Task<ActionResult<GraphicalReportItem>> AddGraphicalConfigLayoutItem([FromQuery] Guid id, [FromBody] AddReportItemSubmission submission)
    {
        var result = await m_analysisModelService.AddGraphicalConfigLayoutItemAsync(id, submission);
        return Ok(result);
    }

    [HttpDelete("model/graphicalconfig/item")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<ActionResult<GraphicalReportItem>> DeleteGraphicalConfigLayoutItem([FromQuery] Guid id)
    {
        await m_analysisModelService.DeleteGraphicalConfigLayoutItemAsync(id);
        return Ok();
    }

    [HttpPut("model/graphicalconfig/item")]
    [ProducesResponseType(typeof(GraphicalReportItem), 200)]
    [Authorize]
    public async Task<ActionResult<GraphicalReportItem>> UpdateGraphicalConfigLayoutItem([FromQuery] Guid id, [FromBody] UpdateReportItemSubmission submission)
    {
        var item = await m_analysisModelService.UpdateGraphicalConfigLayoutItemAsync(id, submission);
        return Ok(item);
    }

    [HttpPut("model/graphicalconfig/item/kpis")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<ActionResult<GraphicalReportItem>> UpdateGraphicalConfigLayoutItemKPIs([FromQuery] Guid id, [FromBody] UpdateKPIsOfGraphicalItemSubmission submission)
    {
        await m_analysisModelService.UpdateGraphicalConfigItemKPIs(id, submission);
        return Ok();
    }

    [HttpPut("model/graphicalconfig/item/properties")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<ActionResult<GraphicalReportItem>> UpdateGraphicalConfigLayoutItemProperties([FromQuery] Guid id, [FromBody] UpdatePropertiesOfGraphicalItemSubmission submission)
    {
        await m_analysisModelService.UpdateGraphicalConfigItemProperties(id, submission);
        return Ok();
    }

    [HttpPut("model/graphicalconfig/layout")]
    [ProducesResponseType(typeof(List<GraphicalReportItemLayout>), 200)]
    [Authorize]
    public async Task<ActionResult<List<GraphicalReportItemLayout>>> UpdateLayout([FromQuery] Guid id, LayoutUpdateSubmission submission)
    {
        var result = await m_analysisModelService.UpdateLayoutAsync(id, submission);
        return Ok(result);
    }

    [HttpPost("model/createreport")]
    [ProducesResponseType(typeof(Report), 200)]
    [Authorize]
    public async Task<ActionResult<Report>> CreateReport([FromBody] CreateReportSubmission submission)
    {
        var report = await m_analysisModelService.CreateReportAsync(submission);
        return Ok(report);
    }

    [HttpDelete("model/deletereport")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<ActionResult> DeleteReport([FromQuery] Guid reportId)
    {
        await m_analysisModelService.DeleteReportAsync(reportId);
        return Ok();
    }

    [HttpGet("report")]
    [ProducesResponseType(typeof(Report), 200)]
    [Authorize]
    public async Task<ActionResult<Report>> GetReportDetails([FromQuery] Guid id)
    {
        var report = await m_analysisModelService.GetReportAsync(id);
        return Ok(report);
    }

    [HttpPost("createmodel")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    [Authorize]
    public async Task<ActionResult<AnalysisModel>> CreateModel(AnalysisModelRequest request)
    {
        var model = await m_analysisModelService.CreateAsync(request);
        return Ok(model);
    }

    [HttpGet("customqueries")]
    [ProducesResponseType(typeof(Query[]), 200)]
    [Authorize]
    public ActionResult<IEnumerable<Query>> GetCustomQueries()
    {
        var queries = m_devOpsProviderService.GetQueries();
        return Ok(queries);
    }

    [HttpGet("requiredQueries")]
    [ProducesResponseType(typeof(string[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<string>>> GetRequiredQueries([FromQuery] Guid modelId)
    {
        var queries = await m_analysisModelService.GetRequiredQueriesAsync(modelId);
        return Ok(queries);
    }

    [HttpGet("customquery")]
    [ProducesResponseType(typeof(Query), 200)]
    [Authorize]
    public ActionResult<IEnumerable<Query>> GetCustomQueries([FromQuery] string queryId)
    {
        var query = m_devOpsProviderService.GetQueryById(queryId);
        return Ok(query);
    }

    [HttpGet("queryparameters")]
    [ProducesResponseType(typeof(QueryParameter[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Query>>> GetCustomQueryParameters([FromQuery] string queryId)
    {
        var parameters = await m_devOpsProviderService.GetQueryParametersAsync(queryId);
        return Ok(parameters);
    }

    [HttpPost("kpifolder")]
    [ProducesResponseType(typeof(KPIFolder), 200)]
    [Authorize]
    public async Task<ActionResult<KPIFolder>> CreateKPIFolder([FromBody] CreateKPIFolderSubmission submission)
    {
        var kpiFolder = await m_kpiService.CreateNewKPIFolderAsync(submission.ModelId, submission.FolderId, submission.Name);
        return Ok(kpiFolder);
    }

    [HttpPut("kpifolder")]
    [ProducesResponseType(typeof(KPIFolder), 200)]
    [Authorize]
    public async Task<ActionResult<KPIFolder>> UpdateKPIFolderName([FromQuery] Guid folderId, [FromBody] UpdateKPIFolderSubmission submission)
    {
        var kpiFolder = await m_kpiService.UpdateKPIFolderAsync(folderId, submission);
        return Ok(kpiFolder);
    }

    [HttpDelete("kpifolder")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<ActionResult> DeleteKPIFolder(Guid folderId)
    {
        await m_kpiService.DeleteKPIFolderAsync(folderId);
        return Ok();
    }

    [HttpPut("kpi/move")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task<ActionResult> MoveKPI([FromBody] MoveKPISubmission submission)
    {
        await m_kpiService.MoveKPIAsync(submission);
        return Ok();
    }

    [HttpPost("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<ActionResult<KPI>> CreateKPI(Guid modelId, Guid? folderId = null)
    {
        var kpi = await m_kpiService.CreateNewKPIAsync(modelId, folderId);
        return Ok(kpi);
    }

    [HttpGet("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<ActionResult<KPI>> GetKPI(Guid id)
    {
        var kpi = await m_kpiService.GetByIdAsync(id);
        return Ok(kpi);
    }

    [HttpPut("kpi")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<ActionResult<KPI>> UpdateKPI([FromBody] KPIUpdate update)
    {
        var kpi = await m_kpiService.UpdateKPIAsync(update);
        return Ok(kpi);
    }

    [HttpPut("kpi/config")]
    [ProducesResponseType(typeof(KPI), 200)]
    [Authorize]
    public async Task<ActionResult<KPIConfigUpdate>> UpdateKPIConfig([FromQuery] Guid id, [FromBody] KPIConfigUpdate update)
    {
        var kpi = await m_kpiService.UpdateKPIConfigAsync(id, update);
        return Ok(kpi);
    }

    [HttpDelete("kpi")]
    [ProducesResponseType(typeof(void), 200)]
    [Authorize]
    public async Task UpdateKPI(Guid id) => await m_kpiService.DeleteKPIAsync(id);

    [HttpPost("kpi/expression")]
    [ProducesResponseType(typeof(Expression), 200)]
    [Authorize]
    public async Task<ActionResult<Expression>> AddExpression([FromQuery] Guid kpiId, [FromBody] ExpressionSubmission expression)
    {
        if (expression.Expression == null)
        {
            throw new ArgumentException("property 'expression' has to be provided!");
        }
        var saved = await m_kpiService.SaveExpressionAsync(kpiId, expression.Expression);
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
        var updated = await m_kpiService.UpdateExpressionAsync(kpiId, expression.Expression);
        return Ok(updated);
    }

}
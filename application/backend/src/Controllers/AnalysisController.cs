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
using backend.Model.Users;
using backend.Services.Security;
using backend.Model.Security;
using System.Runtime.CompilerServices;
using backend.Model.Analysis.Reports;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AnalysisController : Controller
{
    private readonly IAnalysisModelService m_analysisModelService;
    private readonly IKPIService m_kpiService;
    private readonly IDevOpsProviderService m_devOpsProviderService;
    private readonly ISecurityService m_securityService;

    public AnalysisController(
                            IAnalysisModelService analysisModelService,
                            IKPIService kpiService,
                            IDevOpsProviderService devOpsProviderService,
                            ISecurityService securityService)
    {
        m_analysisModelService = analysisModelService;
        m_kpiService = kpiService;
        m_devOpsProviderService = devOpsProviderService;
        m_securityService = securityService;

    }

    [HttpGet("models")]
    [ProducesResponseType(typeof(AnalysisModel[]), 200)]
    public async Task<ActionResult<IEnumerable<AnalysisModel>>> GetMyModels()
    {
        var models = await m_analysisModelService.GetMyModelsAsync();
        return Ok(models);
    }

    [HttpPost("models")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    public async Task<ActionResult<AnalysisModel>> CreateModel(AnalysisModelRequest request)
    {
        var model = await m_analysisModelService.CreateAsync(request);
        return Ok(model);
    }

    [HttpGet("models/{id}")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    public async Task<ActionResult<AnalysisModel>> GetModelById(Guid id)
    {
        await m_securityService.AuthorizeModelAsync(id, Operations.ViewModel, User);
        var model = await m_analysisModelService.GetByIdAsync(id);
        return Ok(model);
    }

    [HttpPut("models/{id}")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    public async Task<ActionResult<AnalysisModel>> UpdateModel(Guid id, [FromBody] AnalysisModelUpdate request)
    {
        await m_securityService.AuthorizeModelAsync(id, Operations.EditModel, User);
        var model = await m_analysisModelService.UpdateAsync(id, request);
        return Ok(model);
    }

    [HttpDelete("models/{id}")]
    [ProducesResponseType(typeof(AnalysisModel), 200)]
    public async Task<ActionResult<AnalysisModel>> DeleteModel(Guid id)
    {
        await m_securityService.AuthorizeModelAsync(id, Operations.DeleteModel, User);
        await m_analysisModelService.DeleteModelAsync(id);
        return Ok();
    }

    [HttpPost("models/{modelId}/graphicalconfig")]
    [ProducesResponseType(typeof(GraphicalConfiguration), 200)]
    public async Task<ActionResult<GraphicalConfiguration>> CreateNewGraphicalConfig(Guid modelId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.CreateGraphicalConfig, User);
        var config = await m_analysisModelService.CreateGraphicalConfigAsync(modelId);
        return Ok(config);
    }

    [HttpGet("models/{modelId}/graphicalconfig/{id}")]
    [ProducesResponseType(typeof(GraphicalConfiguration), 200)]
    public async Task<ActionResult<GraphicalConfiguration>> GetGraphicalConfig(Guid modelId, Guid id)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.ViewGraphicalConfig, User);
        var config = await m_analysisModelService.GetGraphicalConfigAsync(id);
        return Ok(config);
    }

    [HttpPut("models/{modelId}/graphicalconfig/{id}")]
    [ProducesResponseType(typeof(GraphicalConfiguration), 200)]
    public async Task<ActionResult<GraphicalConfiguration>> UpdateGraphicalConfig(Guid modelId, Guid id, [FromBody] GraphicalConfigurationUpdate submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditGraphicalConfig, User);
        var config = await m_analysisModelService.UpdateConfigAsync(id, submission);
        return Ok(config);
    }

    [HttpDelete("models/{modelId}/graphicalconfig/{id}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<ActionResult> DeleteGraphicalConfig(Guid modelId, Guid id)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.DeleteGraphicalConfig, User);

        await m_analysisModelService.DeleteConfigAsync(id);
        return Ok();
    }

    [HttpPost("models/{modelId}/graphicalconfig/{id}/item")]
    [ProducesResponseType(typeof(GraphicalReportItem), 200)]
    public async Task<ActionResult<GraphicalReportItem>> AddGraphicalConfigLayoutItem(Guid modelId, Guid id, [FromBody] AddReportItemSubmission submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.CreateGraphicalItemKPIs, User);

        var result = await m_analysisModelService.AddGraphicalConfigLayoutItemAsync(id, submission);
        return Ok(result);
    }

    [HttpDelete("models/{modelId}/graphicalconfig/{configId}/item/{id}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<ActionResult<GraphicalReportItem>> DeleteGraphicalConfigLayoutItem(Guid modelId, Guid configId, Guid id)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.DeleteGraphicalItem, User);

        await m_analysisModelService.DeleteGraphicalConfigLayoutItemAsync(id);
        return Ok();
    }

    [HttpPut("models/{modelId}/graphicalconfig/{configId}/item/{id}")]
    [ProducesResponseType(typeof(GraphicalReportItem), 200)]
    public async Task<ActionResult<GraphicalReportItem>> UpdateGraphicalConfigLayoutItem(Guid modelId, Guid configId, Guid id, [FromBody] UpdateReportItemSubmission submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditGraphicalItem, User);

        var item = await m_analysisModelService.UpdateGraphicalConfigLayoutItemAsync(id, submission);
        return Ok(item);
    }

    [HttpPut("models/{modelId}/graphicalconfig/{configId}/item/{itemId}/kpis")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<ActionResult<GraphicalReportItem>> UpdateGraphicalConfigLayoutItemKPIs(Guid modelId, Guid configId, Guid itemId, [FromBody] UpdateKPIsOfGraphicalItemSubmission submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditGraphicalItemKPIs, User);

        await m_analysisModelService.UpdateGraphicalConfigItemKPIs(itemId, submission);
        return Ok();
    }

    [HttpPut("models/{modelId}/graphicalconfig/{configId}/item/{itemId}/properties")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<ActionResult<GraphicalReportItem>> UpdateGraphicalConfigLayoutItemProperties(Guid modelId, Guid configId, Guid itemId, [FromBody] UpdatePropertiesOfGraphicalItemSubmission submission)
    {
        ;
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditGraphicalItemProperties, User);

        await m_analysisModelService.UpdateGraphicalConfigItemProperties(itemId, submission);
        return Ok();
    }

    [HttpPut("models/{modelId}/graphicalconfig/{configId}/item/{itemId}/layout")]
    [ProducesResponseType(typeof(List<GraphicalReportItemLayout>), 200)]
    public async Task<ActionResult<List<GraphicalReportItemLayout>>> UpdateLayout(Guid modelId, Guid configId, Guid itemId, LayoutUpdateSubmission submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditGraphicalItemLayout, User);

        var result = await m_analysisModelService.UpdateLayoutAsync(itemId, submission);
        return Ok(result);
    }

    [HttpPost("models/{modelId}/reports")]
    [ProducesResponseType(typeof(Report), 200)]
    public async Task<ActionResult<Report>> CreateReport(Guid modelId, [FromBody] CreateReportSubmission submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.CreateReport, User);
        var report = await m_analysisModelService.CreateReportAsync(modelId, submission);
        return Ok(report);
    }

    [HttpDelete("models/{modelId}/reports/{reportId}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<ActionResult> DeleteReport(Guid modelId, Guid reportId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.DeleteReport, User);

        await m_analysisModelService.DeleteReportAsync(reportId);
        return Ok();
    }

    [HttpGet("models/{modelId}/reports/{reportId}")]
    [ProducesResponseType(typeof(Report), 200)]
    public async Task<ActionResult<Report>> GetReportDetails(Guid modelId, Guid reportId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.ViewReport, User);
        var report = await m_analysisModelService.GetReportAsync(reportId);
        return Ok(report);
    }

    [HttpPost("models/{modelId}/users")]
    [ProducesResponseType(typeof(User), 200)]
    public async Task<ActionResult<User>> AddUserToModel(Guid modelId, [FromBody] AddUserToModelSubmission submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.AddUserToModel, User);

        var user = await m_analysisModelService.AddUserToModelAsync(modelId, submission);
        return Ok(user);
    }

    [HttpPost("models/{modelId}/users/{userId}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<ActionResult<User>> ChangeUserPermissionOnModel(Guid modelId, Guid userId, [FromQuery] ModelPermission permission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.ChangeUserRoleOnModel, User);

        await m_analysisModelService.ChangeUserPermissionOnModelAsync(modelId, userId, permission);
        return Ok();
    }

    [HttpDelete("models/{modelId}/users/{userId}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<ActionResult<User>> RemoveUserFromModel(Guid modelId, Guid userId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.RemoveUserFromModel, User);

        await m_analysisModelService.RemoveUserFromModelAsync(modelId, userId);
        return Ok();
    }

    [HttpGet("customqueries")]
    [ProducesResponseType(typeof(Query[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Query>>> GetCustomQueries()
    {
        var queries = await m_devOpsProviderService.GetQueries();
        return Ok(queries);
    }

    [HttpGet("customqueries/{queryId}")]
    [ProducesResponseType(typeof(Query), 200)]
    [Authorize]
    public ActionResult<IEnumerable<Query>> GetCustomQuery(string queryId)
    {
        var query = m_devOpsProviderService.GetQueryById(queryId);
        return Ok(query);
    }

    [HttpGet("customqueries/{queryId}/queryparameters")]
    [ProducesResponseType(typeof(QueryParameter[]), 200)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Query>>> GetCustomQueryParameters(string queryId)
    {
        var parameters = await m_devOpsProviderService.GetQueryRuntimeParametersAsync(queryId);
        return Ok(parameters);
    }

    [HttpGet("models/{modelId}/requiredQueries")]
    [ProducesResponseType(typeof(string[]), 200)]
    public async Task<ActionResult<IEnumerable<string>>> GetRequiredQueries(Guid modelId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.CreateReport, User);

        var queries = await m_analysisModelService.GetRequiredQueriesAsync(modelId);
        return Ok(queries);
    }

    [HttpPost("models/{modelId}/kpifolders")]
    [ProducesResponseType(typeof(KPIFolder), 200)]
    public async Task<ActionResult<KPIFolder>> CreateKPIFolder(Guid modelId, [FromBody] CreateKPIFolderSubmission submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.CreateKPIFolder, User);
        var kpiFolder = await m_kpiService.CreateNewKPIFolderAsync(modelId, submission.FolderId, submission.Name);
        return Ok(kpiFolder);
    }

    [HttpPut("models/{modelId}/kpifolders/{folderId}")]
    [ProducesResponseType(typeof(KPIFolder), 200)]
    public async Task<ActionResult<KPIFolder>> UpdateKPIFolderName(Guid modelId, Guid folderId, [FromBody] UpdateKPIFolderSubmission submission)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditKPIFolder, User);
        var kpiFolder = await m_kpiService.UpdateKPIFolderAsync(folderId, submission);
        return Ok(kpiFolder);
    }

    [HttpDelete("models/{modelId}/kpifolders/{folderId}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task<ActionResult> DeleteKPIFolder(Guid modelId, Guid folderId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.DeleteKPIFolder, User);

        await m_kpiService.DeleteKPIFolderAsync(folderId);
        return Ok();
    }

    [HttpPost("models/{modelId}/kpis")]
    [ProducesResponseType(typeof(KPI), 200)]
    public async Task<ActionResult<KPI>> CreateKPI(Guid modelId, [FromQuery] Guid? folderId = null)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.CreateKPI, User);

        var kpi = await m_kpiService.CreateNewKPIAsync(modelId, folderId);
        return Ok(kpi);
    }

    [HttpGet("models/{modelId}/kpis/{kpiId}")]
    [ProducesResponseType(typeof(KPI), 200)]
    public async Task<ActionResult<KPI>> GetKPI(Guid modelId, Guid kpiId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.ViewKPI, User);

        var kpi = await m_kpiService.GetByIdAsync(kpiId);
        return Ok(kpi);
    }

    [HttpPut("models/{modelId}/kpis/{kpiId}")]
    [ProducesResponseType(typeof(KPI), 200)]
    public async Task<ActionResult<KPI>> UpdateKPI(Guid modelId, Guid kpiId, [FromBody] KPIUpdate update)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditKPI, User);

        var kpi = await m_kpiService.UpdateKPIAsync(kpiId, update);
        return Ok(kpi);
    }

    [HttpPut("models/{modelId}/kpis/{kpiId}/config")]
    [ProducesResponseType(typeof(KPI), 200)]
    public async Task<ActionResult<KPIConfigUpdate>> UpdateKPIConfig(Guid modelId, Guid kpiId, [FromBody] KPIConfigUpdate update)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditKPI, User);

        var kpi = await m_kpiService.UpdateKPIConfigAsync(kpiId, update);
        return Ok(kpi);
    }

    [HttpDelete("models/{modelId}/kpis/{kpiId}")]
    [ProducesResponseType(typeof(void), 200)]
    public async Task UpdateKPI(Guid modelId, Guid kpiId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.DeleteKPI, User);
        await m_kpiService.DeleteKPIAsync(kpiId);
    }

    [HttpPut("models/{modelId}/kpis/{kpiId}/expression")]
    [ProducesResponseType(typeof(Expression), 200)]
    public async Task<ActionResult<Expression>> AddOrUpdateExpression(Guid modelId, Guid kpiId, [FromBody] ExpressionSubmission expression)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditKPI, User);

        if (expression.Expression == null)
        {
            throw new ArgumentException("property 'expression' has to be provided!");
        }
        var updated = await m_kpiService.UpdateExpressionAsync(kpiId, expression.Expression);
        return Ok(updated);
    }

    [HttpDelete("models/{modelId}/kpis/{kpiId}/expression/condition/{conditionId}")]
    [ProducesResponseType(typeof(Expression), 200)]
    public async Task<ActionResult<Expression>> DeleteKPIExpressionCondition(Guid modelId, Guid kpiId, Guid conditionId)
    {
        await m_securityService.AuthorizeModelAsync(modelId, Operations.EditKPI, User);

        await m_kpiService.DeleteKPIExpressionConditionAsync(kpiId, conditionId);
        return Ok();
    }

}
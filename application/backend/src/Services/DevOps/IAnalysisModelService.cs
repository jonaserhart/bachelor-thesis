using backend.Model.Analysis;
using backend.Model.Analysis.Graphical;
using backend.Model.Rest;
using backend.Model.Users;

namespace backend.Services.DevOps;

public interface IAnalysisModelService
{
    Task<AnalysisModel> GetByIdAsync(Guid id);
    Task<List<AnalysisModel>> GetMyModelsAsync();
    Task<AnalysisModel> CreateAsync(AnalysisModelRequest request);
    Task<AnalysisModel> UpdateAsync(Guid id, AnalysisModelUpdate request);
    Task<GraphicalConfiguration> GetGraphicalConfigAsync(Guid id);
    Task<GraphicalConfiguration> CreateGraphicalConfigAsync(Guid modelId);
    Task<Report> CreateReportAsync(Guid modelId, CreateReportSubmission submission);
    Task<Report> GetReportAsync(Guid id);
    Task DeleteReportAsync(Guid reportId);
    Task<IEnumerable<string>> GetRequiredQueriesAsync(Guid modelId);
    Task<GraphicalConfiguration> UpdateConfigAsync(Guid id, GraphicalConfigurationUpdate submission);
    Task DeleteConfigAsync(Guid id);
    Task<GraphicalReportItemLayout> UpdateLayoutAsync(Guid itemId, LayoutUpdateSubmission submission);
    Task<GraphicalReportItem> AddGraphicalConfigLayoutItemAsync(Guid id, AddReportItemSubmission submission);
    Task DeleteGraphicalConfigLayoutItemAsync(Guid id);
    Task<GraphicalReportItem> UpdateGraphicalConfigLayoutItemAsync(Guid id, UpdateReportItemSubmission submission);
    Task UpdateGraphicalConfigItemProperties(Guid id, UpdatePropertiesOfGraphicalItemSubmission submission);
    Task UpdateGraphicalConfigItemKPIs(Guid id, UpdateKPIsOfGraphicalItemSubmission submission);
    Task<User?> AddUserToModelAsync(Guid modelId, AddUserToModelSubmission submission);
    Task ChangeUserPermissionOnModelAsync(Guid modelId, Guid userId, ModelPermission permission);
}
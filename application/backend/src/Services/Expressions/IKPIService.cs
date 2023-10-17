using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Analysis.KPIs;
using backend.Model.Rest;

namespace backend.Services.Expressions;

public interface IKPIService
{
    Task<KPI> CreateNewKPIAsync(Guid modelId, Guid? folderId);
    Task<KPI> GetByIdAsync(Guid kpiId);
    Task<KPI> UpdateKPIAsync(Guid kpiId, KPIUpdate updated);
    Task<KPIConfigUpdate> UpdateKPIConfigAsync(Guid id, KPIConfigUpdate update);
    Task DeleteKPIAsync(Guid id);
    Task<T> SaveExpressionAsync<T>(Guid addToKPI, T expression) where T : Expression;
    Task<T> UpdateExpressionAsync<T>(Guid kpiId, T expression) where T : Expression;
    Task DeleteKPIExpressionConditionAsync(Guid kpiId, Guid expressionId);
    Task DeleteExpression<T>(T expression) where T : Expression;
    Task<(Dictionary<Guid, object?> result, Dictionary<string, QueryResult> queryResults)> EvaluateKPIs(IEnumerable<KPI> kpis, Dictionary<string, object?> queryParameterValues);
    Task<KPIFolder> CreateNewKPIFolderAsync(Guid modelId, Guid? folderId, string name);
    Task<KPIFolder> GetKPIFolderWithContents(Guid folderId);
    Task<KPIFolder> UpdateKPIFolderAsync(Guid folderId, UpdateKPIFolderSubmission submission);
    Task DeleteKPIFolderAsync(Guid folderId);
    Task MoveKPIFolderAsync(MoveKPISubmission submission);
    Task MoveKPIAsync(MoveKPISubmission submission);
}

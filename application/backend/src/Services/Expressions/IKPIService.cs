using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Rest;

namespace backend.Services.Expressions;

public interface IKPIService
{
    Task<KPI> CreateNewKPIAsync(Guid modelId);
    Task<KPI> GetByIdAsync(Guid kpiId);
    Task<KPI> UpdateKPIAsync(KPIUpdate updated);
    Task<KPIConfigUpdate> UpdateKPIConfigAsync(Guid id, KPIConfigUpdate update);
    Task DeleteKPIAsync(Guid id);
    Task<T> SaveExpressionAsync<T>(Guid addToKPI, T expression) where T : Expression;
    Task<T> UpdateExpressionAsync<T>(Guid kpiId, T expression) where T : Expression;
    Task DeleteExpression<T>(T expression) where T : Expression;
    Task<(Dictionary<Guid, object?> result, Dictionary<string, QueryResult> queryResults)> EvaluateKPIs(IEnumerable<KPI> kpis, Dictionary<string, object?> queryParameterValues);
}

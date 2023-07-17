using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Rest;

namespace backend.Services.Expressions;

public interface IKPIService
{
    Task<KPI> CreateNewKPIAsync(Guid modelId);
    Task<KPI> GetByIdAsync(Guid kpiId);
    Task<KPI> UpdateKPIAsync(KPIUpdate updated);
    Task DeleteKPIAsync(Guid id);
    Task<T> SaveExpressionAsync<T>(Guid addToKPI, T expression) where T : Expression;
    Task<T> UpdateExpressionAsync<T>(Guid kpiId, T expression) where T : Expression;
    Task DeleteExpression<T>(T expression) where T : Expression;
}

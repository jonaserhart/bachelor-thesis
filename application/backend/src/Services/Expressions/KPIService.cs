using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Services.Database;

namespace backend.Services.Expressions;

public class KPIService : IKPIService
{
    private readonly DataContext _context;

    public KPIService(DataContext context)
    {
        _context = context;
    }

    public async Task<KPI> CreateNewKPIAsync(Guid modelId)
    {
        var model = await _context.GetByIdOrThrowAsync<AnalysisModel>(modelId);
        var newKPI = new KPI
        {
            Name = "New KPI",
            Expression = new NumericValueExpression
            {
                Value = 12,
                Type = Model.Enum.ExpressionType.Value
            }
        };
        model.KPIs.Add(newKPI);
        await _context.SaveChangesAsync();
        return newKPI;
    }

    public async Task<KPI> GetByIdAsync(Guid kpiId)
    {
        var found = await _context.GetByIdOrThrowAsync<KPI>(kpiId);
        await _context.Entry(found).Reference(x => x.Expression).LoadAsync();
        return found;
    }

    public async Task<KPI> UpdateKPIAsync(KPIUpdate updated)
    {
        var found = await _context.GetByIdOrThrowAsync<KPI>(updated.Id);
        found.Name = updated.Name;

        await _context.SaveChangesAsync();
        return found;
    }

    public async Task DeleteKPIAsync(Guid id)
    {
        var found = await _context.GetByIdOrThrowAsync<KPI>(id);
        _context.Remove(found);
        await _context.SaveChangesAsync();
    }

    public async Task<T> SaveExpressionAsync<T>(Guid addToKPI, T expression) where T : Expression
    {
        var kpi = await _context.GetByIdOrThrowAsync<KPI>(addToKPI);
        _context.Add<T>(expression);
        kpi.Expression = expression;
        await _context.SaveChangesAsync();
        return expression;
    }

    public async Task<T> UpdateExpressionAsync<T>(Guid kpiId, T expression) where T : Expression
    {
        var entry = await _context.FindAsync<T>(expression.Id);
        if (entry == null)
        {
            await SaveExpressionAsync(kpiId, expression);
        }
        return await UpdateExpressionAsync(expression);
    }

    private async Task<T> UpdateExpressionAsync<T>(T expression) where T : Expression
    {

        switch (expression)
        {
            case MathOperationExpression mathExpr:
                {
                    if (mathExpr.Left != null && mathExpr.Right != null)
                    {
                        var left = await UpdateExpressionAsync(mathExpr.Left);
                        var right = await UpdateExpressionAsync(mathExpr.Right);

                        var mathExpression = (await _context.GetByIdOrThrowAsync<T>(mathExpr.Id)) as MathOperationExpression;

                        await _context.Entry(mathExpression!).Reference(x => x.Left).LoadAsync();
                        await _context.Entry(mathExpression!).Reference(x => x.Right).LoadAsync();

                        return (mathExpression as T)!;
                    }
                    else
                    {
                        throw new ExpressionSaveException();
                    }
                }
            case AggregateExpression aggregateExpr:
                {
                    if (aggregateExpr.FieldExpression != null)
                    {
                        var fieldExpression = await UpdateExpressionAsync(aggregateExpr.FieldExpression);
                        return (aggregateExpr as T)!;
                    }
                    else
                    {
                        throw new ExpressionSaveException();
                    }
                }
            case FieldExpression fieldExpr:
                {
                    var fieldExpression = await _context.GetByIdOrThrowAsync<FieldExpression>(fieldExpr.Id);
                    fieldExpression.Field = fieldExpr.Field;
                    fieldExpression.QueryId = fieldExpr.QueryId;

                    await _context.SaveChangesAsync();
                    return (fieldExpression as T)!;
                }
            case NumericValueExpression valExpr:
                {
                    var valueExpression = await _context.GetByIdOrThrowAsync<NumericValueExpression>(valExpr.Id);
                    valueExpression.Value = valExpr.Value;

                    await _context.SaveChangesAsync();
                    return (valueExpression as T)!;
                }
            case CountIfExpression countIfExpr:
                {
                    var countIfExpression = await _context.GetByIdOrThrowAsync<CountIfExpression>(countIfExpr.Id);

                    countIfExpression.Field = countIfExpr.Field;
                    countIfExpression.Operator = countIfExpr.Operator;
                    countIfExpression.Value = countIfExpr.Value;
                    countIfExpression.QueryId = countIfExpr.QueryId;

                    await _context.SaveChangesAsync();
                    return (countIfExpression as T)!;
                }
            default: throw new ExpressionSaveException();
        }
    }

    public async Task DeleteExpression<T>(T expression) where T : Expression
    {
        _context.Remove<T>(expression);
        await _context.SaveChangesAsync();
    }
}

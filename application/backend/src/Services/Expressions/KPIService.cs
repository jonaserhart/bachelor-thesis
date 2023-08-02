using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Services.Database;
using backend.Services.DevOps;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace backend.Services.Expressions;

public class KPIService : IKPIService
{
    private readonly DataContext _context;
    private readonly IDevOpsProviderService _devopsProviderService;
    private readonly ILogger<KPIService> _logger;

    public KPIService(DataContext context, IDevOpsProviderService devOpsProviderService, ILogger<KPIService> logger)
    {
        _context = context;
        _devopsProviderService = devOpsProviderService;
        _logger = logger;
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

    private object ConvertExpression(Type tExisting, Type tNew, object existingExpression)
    {
        var newExpression = Activator.CreateInstance(tNew);
        if (newExpression == null)
            throw new ExpressionSaveException($"Could not dynamically create expression of type {tNew}");

        var commonProperties = typeof(Expression).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                 .Where(p => p.CanRead && p.CanWrite)
                                                 .ToDictionary(p => p.Name, p => p.GetValue(existingExpression));

        foreach (var prop in commonProperties)
        {
            if (prop.Value != null && prop.Key != "Type")
            {
                tNew.GetProperty(prop.Key)?.SetValue(newExpression, prop.Value);
            }
        }

        return newExpression;
    }

    private async Task<T> UpdateExpressionAsync<T>(T expression) where T : Expression
    {
        Expression existing;
        Expression? newExpression;

        existing = await _context.GetByIdOrThrowAsync<T>(expression.Id);

        // convert expression if needed
        newExpression = ConvertExpression(existing.GetType(), expression.GetType(), existing) as T;


        if (newExpression == null)
            throw new ExpressionSaveException($"Could not update expression {existing.Id}");

        newExpression.Type = expression.Type;

        // copy rest of properties specific to the expression type
        switch (expression)
        {
            case MathOperationExpression mathExpr:
                {

                    if (mathExpr.LeftId == null || mathExpr.RightId == null)
                        throw new ExpressionSaveException("Requested to c");

                    var left = await _context.GetByIdOrThrowAsync<KPI>(mathExpr.LeftId ?? Guid.NewGuid());
                    var right = await _context.GetByIdOrThrowAsync<KPI>(mathExpr.RightId ?? Guid.NewGuid());

                    (newExpression as MathOperationExpression)!.Left = left;
                    (newExpression as MathOperationExpression)!.Right = right;

                    break;
                }
            case AggregateExpression aggregateExpr:
                {
                    if (aggregateExpr == null)
                        throw new ExpressionSaveException();

                    (newExpression as AggregateExpression)!.Field = aggregateExpr.Field;
                    (newExpression as AggregateExpression)!.QueryId = aggregateExpr.QueryId;
                    break;
                }
            case NumericValueExpression numericValueExpr:
                {
                    if (numericValueExpr == null)
                        throw new ExpressionSaveException();

                    (newExpression as NumericValueExpression)!.Value = numericValueExpr.Value;
                    break;
                }
            case PlainQueryExpression plainExpr:
                {
                    if (plainExpr == null)
                        throw new ExpressionSaveException();

                    (newExpression as PlainQueryExpression)!.QueryId = plainExpr.QueryId;
                    break;
                }
            case CountIfExpression countIfExpr:
                {
                    if (countIfExpr == null)
                        throw new ExpressionSaveException();

                    (newExpression as CountIfExpression)!.Field = countIfExpr.Field;
                    (newExpression as CountIfExpression)!.Operator = countIfExpr.Operator;
                    (newExpression as CountIfExpression)!.CompareValue = countIfExpr.CompareValue;
                    (newExpression as CountIfExpression)!.QueryId = countIfExpr.QueryId;
                    break;
                }
            case CountExpression countExpr:
                {
                    if (countExpr == null)
                        throw new ExpressionSaveException();

                    (newExpression as CountExpression)!.QueryId = countExpr.QueryId;
                    break;
                }
            default: throw new ExpressionSaveException();
        }

        _context.Entry(existing).State = EntityState.Detached;
        _context.Expressions.Update(newExpression);
        await _context.SaveChangesAsync();
        return (newExpression as T)!;
    }

    public async Task DeleteExpression<T>(T expression) where T : Expression
    {
        _context.Remove<T>(expression);
        await _context.SaveChangesAsync();
    }

    public async Task<(Dictionary<Guid, object?> result, Dictionary<string, QueryResult> queryResults)> EvaluateKPIs(IEnumerable<KPI> kpis, Dictionary<string, object?> queryParameterValues)
    {
        _logger.LogDebug($"Evaluating KPIs...");
        var queries = kpis
            .Where(x => x != null && x.Expression != null)
            .SelectMany(x => x.Expression!.GetRequiredQueries())
            .Distinct();

        var queriesAndResults = new Dictionary<string, QueryResult>();
        foreach (var query in queries)
        {
            _logger.LogDebug($"Executing query {query}...");
            var queryResult = await _devopsProviderService.ExecuteQueryAsync(query, queryParameterValues);
            _logger.LogDebug($"Got result: {queryResult.Value} of type {queryResult.Type}...");
            queriesAndResults.Add(query, queryResult);
        }

        var result = new Dictionary<Guid, object?>();

        foreach (var kpi in kpis)
        {
            var kpiValue = kpi.Expression?.Evaluate(queriesAndResults);
            if (kpiValue != null)
            {
                result.Add(kpi.Id, kpiValue);
            }
        }

        return (result, queriesAndResults);
    }

    public async Task<KPIConfigUpdate> UpdateKPIConfigAsync(Guid id, KPIConfigUpdate update)
    {
        var kpi = await GetByIdAsync(id);

        kpi.AcceptableValues = update.AcceptableValues;
        kpi.ShowInReport = update.ShowInReport;
        kpi.Unit = update.Unit;

        await _context.SaveChangesAsync();

        return update;
    }
}

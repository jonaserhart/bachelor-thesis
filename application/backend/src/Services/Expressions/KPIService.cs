using backend.Model.Analysis;
using backend.Model.Analysis.Expressions;
using backend.Model.Analysis.KPIs;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Services.Database;
using backend.Services.DevOps;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Reflection;

namespace backend.Services.Expressions;

public class KPIService : IKPIService
{
    private readonly DataContext m_context;
    private readonly IDevOpsProviderService m_devopsProviderService;
    private readonly ILogger<KPIService> m_logger;

    public KPIService(DataContext context, IDevOpsProviderService devOpsProviderService, ILogger<KPIService> logger)
    {
        m_context = context;
        m_devopsProviderService = devOpsProviderService;
        m_logger = logger;
    }

    public async Task<KPI> GetByIdAsync(Guid kpiId)
    {
        var found = await m_context.GetByIdOrThrowAsync<KPI>(kpiId);
        await m_context.Entry(found)
            .Reference(x => x.Expression)
            .Query()
            .LoadAsync();
        return found;
    }

    public async Task<KPI> UpdateKPIAsync(Guid kpiId, KPIUpdate updated)
    {
        var found = await m_context.GetByIdOrThrowAsync<KPI>(kpiId);
        found.Name = updated.Name;

        await m_context.SaveChangesAsync();
        return found;
    }

    public async Task DeleteKPIAsync(Guid id)
    {
        var found = await m_context.GetByIdOrThrowAsync<KPI>(id);
        m_context.Remove(found);
        await m_context.SaveChangesAsync();
    }

    public async Task<T> SaveExpressionAsync<T>(Guid addToKPI, T expression) where T : Expression
    {
        var kpi = await m_context.GetByIdOrThrowAsync<KPI>(addToKPI);
        m_context.Add(expression);
        kpi.Expression = expression;
        await m_context.SaveChangesAsync();
        return expression;
    }

    public async Task<T> UpdateExpressionAsync<T>(Guid kpiId, T expression) where T : Expression
    {
        var entry = await m_context.FindAsync<T>(expression.Id);
        if (entry == null)
        {
            await SaveExpressionAsync(kpiId, expression);
        }
        return await UpdateExpressionAsync(expression);
    }

    private object ConvertExpression(Type tNew, object existingExpression)
    {
        var newExpression = Activator.CreateInstance(tNew)
            ?? throw new ExpressionSaveException($"Could not dynamically create expression of type {tNew}");

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

    private static void UpdateConditions(DataContext context, DoIfMultipleExpression leftExpression, DoIfMultipleExpression rightExpression)
    {
        var rightConditionsDict = rightExpression.Conditions.Where(x => Guid.Empty != x.Id).ToDictionary(c => c.Id);

        for (var i = leftExpression.Conditions.Count - 1; i >= 0; i--)
        {
            var leftCondition = leftExpression.Conditions[i];
            if (rightConditionsDict.TryGetValue(leftCondition.Id, out var matchingRightCondition))
            {
                leftCondition.Operator = matchingRightCondition.Operator;
                leftCondition.CompareValue = matchingRightCondition.CompareValue;
                leftCondition.Field = matchingRightCondition.Field;
            }
            else
            {
                var toRemove = leftExpression.Conditions[i];
                context.ExpressionConditions.Remove(toRemove);
                leftExpression.Conditions.RemoveAt(i);
            }
        }

        foreach (var rightCondition in rightExpression.Conditions)
        {
            if (!leftExpression.Conditions.Any(c => c.Id == rightCondition.Id) || rightCondition.Id == Guid.Empty)
            {
                leftExpression.Conditions.Add(new Condition
                {
                    Operator = rightCondition.Operator,
                    CompareValue = rightCondition.CompareValue,
                    Field = rightCondition.Field
                });
            }
        }
    }

    private async Task<T> UpdateExpressionAsync<T>(T expression) where T : Expression
    {
        Expression existing;
        Expression? existingExpressionOfType;

        existing = await m_context.GetByIdOrThrowAsync<T>(expression.Id);

        // convert expression if needed
        existingExpressionOfType = ConvertExpression(expression.GetType(), existing) as T;

        if (existingExpressionOfType == null)
            throw new ExpressionSaveException($"Could not update expression {existing.Id}");

        existingExpressionOfType.Type = expression.Type;

        // copy rest of properties specific to the expression type
        switch (expression)
        {
            case MathOperationExpression mathExpr:
                {

                    if (mathExpr.LeftId == null || mathExpr.RightId == null)
                        throw new ExpressionSaveException("Requested to c");

                    var left = await m_context.GetByIdOrThrowAsync<KPI>(mathExpr.LeftId ?? Guid.NewGuid());
                    var right = await m_context.GetByIdOrThrowAsync<KPI>(mathExpr.RightId ?? Guid.NewGuid());

                    (existingExpressionOfType as MathOperationExpression)!.Left = left;
                    (existingExpressionOfType as MathOperationExpression)!.Right = right;

                    break;
                }
            case AggregateExpression aggregateExpr:
                {
                    if (aggregateExpr == null)
                        throw new ExpressionSaveException();

                    (existingExpressionOfType as AggregateExpression)!.Field = aggregateExpr.Field;
                    (existingExpressionOfType as AggregateExpression)!.QueryId = aggregateExpr.QueryId;
                    break;
                }
            case NumericValueExpression numericValueExpr:
                {
                    if (numericValueExpr == null)
                        throw new ExpressionSaveException();

                    (existingExpressionOfType as NumericValueExpression)!.Value = numericValueExpr.Value;
                    break;
                }
            case PlainQueryExpression plainExpr:
                {
                    if (plainExpr == null)
                        throw new ExpressionSaveException();

                    (existingExpressionOfType as PlainQueryExpression)!.QueryId = plainExpr.QueryId;
                    break;
                }
            case CountIfExpression countIfExpr:
                {
                    if (countIfExpr == null)
                        throw new ExpressionSaveException();

                    (existingExpressionOfType as CountIfExpression)!.Field = countIfExpr.Field;
                    (existingExpressionOfType as CountIfExpression)!.Operator = countIfExpr.Operator;
                    (existingExpressionOfType as CountIfExpression)!.CompareValue = countIfExpr.CompareValue;
                    (existingExpressionOfType as CountIfExpression)!.QueryId = countIfExpr.QueryId;
                    break;
                }
            case CountExpression countExpr:
                {
                    if (countExpr == null)
                        throw new ExpressionSaveException();

                    (existingExpressionOfType as CountExpression)!.QueryId = countExpr.QueryId;
                    break;
                }
            case DoIfMultipleExpression doIfMultipleExpression:
                {
                    if (doIfMultipleExpression == null)
                        throw new ExpressionSaveException();

                    UpdateConditions(m_context, (existingExpressionOfType as DoIfMultipleExpression)!, doIfMultipleExpression);
                    (existingExpressionOfType as DoIfMultipleExpression)!.Connection = doIfMultipleExpression.Connection;
                    (existingExpressionOfType as DoIfMultipleExpression)!.QueryId = doIfMultipleExpression.QueryId;
                    (existingExpressionOfType as DoIfMultipleExpression)!.ExtractField = doIfMultipleExpression.ExtractField;
                    break;
                }
            default: throw new ExpressionSaveException();
        }

        m_context.Entry(existing).State = EntityState.Detached;
        m_context.Expressions.Update(existingExpressionOfType);
        await m_context.SaveChangesAsync();
        return (existingExpressionOfType as T)!;
    }

    public async Task DeleteExpression<T>(T expression) where T : Expression
    {
        m_context.Remove<T>(expression);
        await m_context.SaveChangesAsync();
    }

    public async Task<(Dictionary<Guid, object?> result, Dictionary<string, QueryResult> queryResults)> EvaluateKPIs(IEnumerable<KPI> kpis, Dictionary<string, object?> queryParameterValues)
    {
        foreach (var kpi in kpis)
        {
            // load related data
            await m_context.Entry(kpi).Reference(x => x.Expression).LoadAsync();
        }

        m_logger.LogDebug($"Evaluating KPIs...");
        var queries = kpis
            .Where(x => x != null && x.Expression != null)
            .SelectMany(x => x.Expression!.GetRequiredQueries())
            .Distinct();

        var queriesAndResults = new Dictionary<string, QueryResult>();
        foreach (var query in queries)
        {
            m_logger.LogDebug($"Executing query {query}...");
            var queryResult = await m_devopsProviderService.ExecuteQueryAsync(query, queryParameterValues);
            m_logger.LogDebug($"Got result: {queryResult.Value} of type {queryResult.Type}...");
            queriesAndResults.Add(query, queryResult);
        }

        var result = new Dictionary<Guid, object?>();

        foreach (var kpi in kpis)
        {
            m_logger.LogDebug($"Evaluating KPI ${kpi.Name}");
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

        await m_context.SaveChangesAsync();

        return update;
    }

    public async Task<KPI> CreateNewKPIAsync(Guid modelId, Guid? folderId)
    {

        var model = await m_context.GetByIdOrThrowAsync<AnalysisModel>(modelId);
        var newKPI = new KPI
        {
            Name = "New KPI",
            Expression = new NumericValueExpression
            {
                Value = 12,
                Type = Model.Enum.ExpressionType.Value
            }
        };

        if (folderId != null)
        {
            var folder = await m_context.GetByIdOrThrowAsync<KPIFolder>(folderId!);
            folder.KPIs.Add(newKPI);
        }
        else
        {
            model.KPIs.Add(newKPI);
        }

        await m_context.SaveChangesAsync();
        return newKPI;
    }

    public async Task<KPIFolder> CreateNewKPIFolderAsync(Guid modelId, Guid? folderId, string name)
    {
        var model = await m_context.GetByIdOrThrowAsync<AnalysisModel>(modelId);
        var newKPIFolder = new KPIFolder
        {
            Name = name,
        };

        if (folderId != null)
        {
            var folder = await m_context.GetByIdOrThrowAsync<KPIFolder>(folderId!);
            await m_context.Entry(folder).Collection(x => x.SubFolders).LoadAsync();
            folder.SubFolders.Add(newKPIFolder);
        }
        else
        {
            await m_context.Entry(model).Collection(x => x.KPIFolders).LoadAsync();
            model.KPIFolders.Add(newKPIFolder);
        }

        await m_context.SaveChangesAsync();
        return newKPIFolder;
    }

    public async Task<KPIFolder> GetKPIFolderWithContents(Guid folderId)
    {
        var folder = await m_context.GetByIdOrThrowAsync<KPIFolder>(folderId!);
        await m_context.Entry(folder).Collection(x => x.SubFolders).Query().LoadAsync();
        await m_context.Entry(folder).Collection(x => x.KPIs)
            .Query()
            .Include(x => x.Expression).LoadAsync();
        return folder;
    }

    public async Task<KPIFolder> UpdateKPIFolderAsync(Guid folderId, UpdateKPIFolderSubmission submission)
    {
        if (string.IsNullOrEmpty(submission.Name))
        {
            throw new BadRequestException("Name of folder cannot be empty.");
        }
        var folder = await m_context.GetByIdOrThrowAsync<KPIFolder>(folderId!);
        folder.Name = submission.Name;
        await m_context.SaveChangesAsync();
        return folder;
    }

    public async Task DeleteKPIFolderAsync(Guid folderId)
    {
        var folder = await m_context.GetByIdOrThrowAsync<KPIFolder>(folderId);
        m_context.KPIFolders.Remove(folder);

        await m_context.SaveChangesAsync();
    }

    public async Task MoveKPIFolderAsync(MoveKPISubmission submission)
    {
        var folder = await m_context.GetByIdOrThrowAsync<KPIFolder>(submission.Id);

        if (submission.MoveToFolder != null)
        {
            if (submission.MoveToModel == submission.Id)
            {
                throw new BadRequestException("Cannot move folder into itself you joker.");
            }
            var folderToMoveTo = await m_context.GetByIdOrThrowAsync<KPIFolder>(submission.MoveToFolder);
            folder.ParentFolder = folderToMoveTo;

            await m_context.SaveChangesAsync();
            return;
        }
        if (submission.MoveToModel != null)
        {
            var modelToMoveTo = await m_context.GetByIdOrThrowAsync<AnalysisModel>(submission.MoveToModel);
            folder.ParentFolder = null;
            folder.AnalysisModel = modelToMoveTo;

            await m_context.SaveChangesAsync();
            return;
        }
        throw new BadRequestException($"Please provide either folder or model to move KPI folder '{folder.Name}' to");

    }

    public async Task MoveKPIAsync(MoveKPISubmission submission)
    {
        var kpi = await m_context.GetByIdOrThrowAsync<KPI>(submission.Id);
        if (submission.MoveToFolder != null)
        {
            var folderToMoveTo = await m_context.GetByIdOrThrowAsync<KPIFolder>(submission.MoveToFolder);
            kpi.Folder = folderToMoveTo;

            await m_context.SaveChangesAsync();
            return;
        }
        if (submission.MoveToModel != null)
        {
            var modelToMoveTo = await m_context.GetByIdOrThrowAsync<AnalysisModel>(submission.MoveToModel);
            kpi.Folder = null;
            kpi.AnalysisModel = modelToMoveTo;

            await m_context.SaveChangesAsync();
            return;
        }
        throw new BadRequestException($"Please provide either folder or model to move KPI '{kpi.Name}' to");
    }
}

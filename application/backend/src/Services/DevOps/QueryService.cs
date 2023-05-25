using backend.Model.Analysis;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Services.API;
using backend.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.DevOps;

public class QueryService : IQueryService
{
    private readonly DataContext _context;
    private readonly IApiClientFactory _apiClientFactory;

    public QueryService(DataContext context, IApiClientFactory apiClientFactory)
    {
        _context = context;
        _apiClientFactory = apiClientFactory;
    }

    public async Task<Query> GetQueryByIdAsync(Guid queryId)
    {
        var query = await _context.Queries.Include(x => x.Where).Include(x => x.Select).FirstOrDefaultAsync(x => x.Id == queryId);

        if (query == null)
            throw new DbKeyNotFoundException(queryId, typeof(Query));

        return query;
    }

    public async Task<IEnumerable<QueryResponse>> GetQueriesAsync(string projectId)
    {
        using var client = await _apiClientFactory.GetApiClientAsync();
        var queries = await client.GetQueriesAsync(projectId);
        return queries;
    }

    public async Task<Query> CreateQueryFromDevOps(Guid modelId, Guid queryId)
    {
        var model = await _context.AnalysisModels.FindAsync(modelId);

        if (model == null)
            throw new DbKeyNotFoundException(modelId, typeof(AnalysisModel));

        using var client = await _apiClientFactory.GetApiClientAsync();
        var query = await client.GetQueryByIdAsync(model.ProjectId?.ToString() ?? string.Empty, queryId.ToString());
        model.Queries.Add(query);
        await _context.SaveChangesAsync();

        return query;
    }

    public async Task<Query> GetQueryWithClauses(Guid queryId)
    {
        var query = _context.Queries
            .Include(x => x.Select)
            .Include(q => q.Where)
            .FirstOrDefault(q => q.Id == queryId);

        if (query == null)
        {
            throw new DbKeyNotFoundException(queryId, typeof(Query));
        }

        if (query.Where != null)
        {
            await LoadNestedClauses(query.Where);
        }

        return query;
    }

    private async Task LoadNestedClauses(Clause clause)
    {
        if (clause != null)
        {
            await _context.Entry(clause).Collection(c => c.Clauses).LoadAsync();

            foreach (var nestedClause in clause.Clauses)
            {
                await LoadNestedClauses(nestedClause);
            }
        }
    }

    public async Task<QueryChange> UpdateQueryAsync(QueryChange queryChange)
    {
        if (queryChange == null || string.IsNullOrEmpty(queryChange.Name))
        {
            throw new BadRequestException($"Name argument has to be provided.");
        }

        var query = await _context.Queries.FindAsync(queryChange.Id);

        if (query == null)
        {
            throw new DbKeyNotFoundException(queryChange.Id, typeof(Query));
        }

        query.Name = queryChange.Name;

        await _context.SaveChangesAsync();
        return queryChange;
    }
}
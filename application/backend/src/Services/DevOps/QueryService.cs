using backend.Model.Analysis;
using backend.Model.Analysis.Queries;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Services.API;
using backend.Services.Database;
using backend.Services.DynamicQuery;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.DevOps;

public class QueryService : IQueryService
{
    private readonly DataContext _context;
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IDynamicQueryService _dynamicQueryService;

    public QueryService(DataContext context, IApiClientFactory apiClientFactory, IDynamicQueryService dynamicQueryService)
    {
        _context = context;
        _apiClientFactory = apiClientFactory;
        _dynamicQueryService = dynamicQueryService;
    }

    public IEnumerable<BaseQuerySchema> GetBaseQuerySchemas()
    {
        return _dynamicQueryService.GetSchemas();
    }

    public async Task<IEnumerable<QueryParameter>> GetQueryCreateParametersAsync(Guid schemaId)
    {
        var schema = _dynamicQueryService.GetByIdOrThrow(schemaId);
        return await schema.GetCreateQueryParametersAsync();
    }

    public async Task<Model.Analysis.Queries.Query> CreateQueryAsync(Guid schemaId, List<QueryParameterValue> queryParameterValue)
    {
        var schema = _dynamicQueryService.GetByIdOrThrow(schemaId);
        var query = await schema.CreateQueryAsync(queryParameterValue);
        return query;
    }

    public async Task<IEnumerable<QueryParameter>> GetQueryRuntimeParametersAsync(Guid schemaId, Guid queryId)
    {
        var schema = _dynamicQueryService.GetByIdOrThrow(schemaId);
        // TODO: get form db
        Model.Analysis.Queries.Query query = new Model.Analysis.Queries.Query();
        var parameters = await schema.GetRuntimeParametersAsync(query);
        return parameters;
    }

    public async Task<object?> ExecuteQueryAsync(Guid schemaId, Guid queryId, List<QueryParameterValue> runtimeParams)
    {
        var schema = _dynamicQueryService.GetByIdOrThrow(schemaId);
        // TODO: get form db
        Model.Analysis.Queries.Query query = new Model.Analysis.Queries.Query();
        var value = await schema.ExecuteQueryAsync(query, runtimeParams);
        return value;
    }

    public async Task<Model.Analysis.Query> GetQueryByIdAsync(Guid queryId)
    {
        var query = await _context.Queries.Include(x => x.Where).Include(x => x.Select).FirstOrDefaultAsync(x => x.Id == queryId);

        if (query == null)
            throw new DbKeyNotFoundException(queryId, typeof(Model.Analysis.Query));

        return query;
    }

    public async Task<IEnumerable<QueryResponse>> GetQueriesAsync(string projectId)
    {
        using var client = await _apiClientFactory.GetApiClientAsync();
        var queries = await client.GetQueriesAsync(projectId);
        return queries;
    }

    public async Task<Model.Analysis.Query> CreateQueryFromDevOps(Guid modelId, Guid queryId)
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

    public async Task<Model.Analysis.Query> GetQueryWithClausesAsync(Guid queryId)
    {
        var query = _context.Queries
            .Include(x => x.Select)
            .Include(q => q.Where)
            .FirstOrDefault(q => q.Id == queryId);

        if (query == null)
        {
            throw new DbKeyNotFoundException(queryId, typeof(Model.Analysis.Query));
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
            throw new DbKeyNotFoundException(queryChange.Id, typeof(Model.Analysis.Query));
        }

        query.Name = queryChange.Name;

        await _context.SaveChangesAsync();
        return queryChange;
    }
}
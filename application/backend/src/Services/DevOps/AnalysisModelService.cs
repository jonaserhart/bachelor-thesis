using backend.Model.Analysis;
using backend.Model.Analysis.WorkItems;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Model.Users;
using backend.Services.API;
using backend.Services.Database;
using backend.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.DevOps;

public class AnalysisModelService : IAnalysisModelService
{
    private readonly DataContext _context;
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IUserService _userService;
    private readonly IQueryService _queryService;

    public AnalysisModelService(DataContext context, IUserService userService, IApiClientFactory apiClientFactory, IQueryService queryService)
    {
        _context = context;
        _userService = userService;
        _apiClientFactory = apiClientFactory;
        _queryService = queryService;
    }

    private IQueryable<AnalysisModel> IncludeUsersProjectsQueriesTeams(IQueryable<AnalysisModel> query)
    {
        return query
            .Include(x => x.ModelUsers)
            .Include(x => x.Project)
            .Include(x => x.Queries)
            .ThenInclude(x => x.Select)
            .Include(x => x.Queries)
            .ThenInclude(x => x.Where)
            .Include(x => x.Team)
            .Include(x => x.KPIs)
            .ThenInclude(x => x.Expression);
    }

    public async Task<AnalysisModel?> GetByIdAsync(Guid id)
    {
        return await IncludeUsersProjectsQueriesTeams(_context.AnalysisModels)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        var client = await _apiClientFactory.GetApiClientAsync();
        return await client.GetProjectsAsync();
    }

    public async Task<List<AnalysisModel>> GetMyModelsAsync()
    {
        var user = await _userService.GetSelfAsync();
        var models = user.UserModels.Select(x => x.ModelId);
        return await IncludeUsersProjectsQueriesTeams(_context.AnalysisModels)
            .Where(x => models.Contains(x.Id)).ToListAsync();
    }

    public async Task<AnalysisModel> CreateAsync(AnalysisModelRequest request)
    {
        if (request.Project == null)
            throw new BadRequestException("'Project' cannot be empty when creating a model.");

        var project = await _context.Projects.FindAsync(request.Project.Id);
        if (project == null)
            project = Project.From(request.Project);

        var model = new AnalysisModel
        {
            Name = request.Name,
            Project = project
        };

        await _context.AnalysisModels.AddAsync(model);
        await _context.SaveChangesAsync();

        var user = await _userService.GetSelfAsync();

        var userModel = new UserModel
        {
            Model = model,
            User = user,
        };

        await _context.UserModels.AddAsync(userModel);
        await _context.SaveChangesAsync();

        return model;
    }

    public async Task<AnalysisModel> UpdateAsync(Guid id, AnalysisModelUpdate request)
    {
        var model = await IncludeUsersProjectsQueriesTeams(_context.AnalysisModels).FirstOrDefaultAsync(x => x.Id == id);
        if (model == null)
            throw new DbKeyNotFoundException(id, typeof(AnalysisModel));

        model.Name = request.Name;

        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<IEnumerable<Team>> GetTeamsAsync(string projectId)
    {
        var client = await _apiClientFactory.GetApiClientAsync();
        return await client.GetTeamsAsync(projectId);
    }

    public async Task<IEnumerable<Iteration>> GetIterationsAsync(string projectId, string teamId)
    {
        var client = await _apiClientFactory.GetApiClientAsync();
        var iterations = await client.GetIterationsAsync(projectId, teamId);
        return iterations;
    }

    public async Task<IEnumerable<FieldInfo>> GetFieldInfosAsync(string projectId)
    {
        var client = await _apiClientFactory.GetApiClientAsync();
        var fields = await client.GetFieldInfosAsync(projectId);

        return fields;
    }

    public async Task<IEnumerable<Workitem>> GetWorkitemsAsync(string projectId, Iteration iteration, Guid queryId, DateTime? asOf = null)
    {
        var client = await _apiClientFactory.GetApiClientAsync();
        var query = await _queryService.GetQueryWithClausesAsync(queryId);
        var items = await client.GetWorkitemsAsync(projectId, iteration, query, asOf);
        return items;
    }
}
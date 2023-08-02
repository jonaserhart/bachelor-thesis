using backend.Model.Analysis;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Model.Users;
using backend.Services.API;
using backend.Services.Database;
using backend.Services.Expressions;
using backend.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.DevOps;

public class AnalysisModelService : IAnalysisModelService
{
    private readonly DataContext _context;
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IUserService _userService;
    private readonly IKPIService _kpiService;
    private readonly ILogger<AnalysisModelService> _logger;

    public AnalysisModelService(
        DataContext context, IUserService userService, IApiClientFactory apiClientFactory, IKPIService kpiService, ILogger<AnalysisModelService> logger)
    {
        _context = context;
        _userService = userService;
        _apiClientFactory = apiClientFactory;
        _kpiService = kpiService;
        _logger = logger;
    }

    public async Task<AnalysisModel?> GetByIdAsync(Guid id)
    {
        var model = await _context.GetByIdOrThrowAsync<AnalysisModel>(id);

        await _context.Entry(model).Collection(x => x.KPIs)
            .Query()
            .Include(x => x.Expression)
            .LoadAsync();
        await _context.Entry(model).Collection(x => x.ModelUsers).LoadAsync();
        await _context.Entry(model).Collection(x => x.Reports).LoadAsync();
        return model;
    }

    public async Task<List<AnalysisModel>> GetMyModelsAsync()
    {
        var user = await _userService.GetSelfAsync();
        var models = user.UserModels.Select(x => x.ModelId);
        return await _context.AnalysisModels
                .Include(x => x.KPIs)
                .Include(x => x.ModelUsers)
                .Include(x => x.Reports)
            .Where(x => models.Contains(x.Id)).ToListAsync();
    }

    public async Task<AnalysisModel> CreateAsync(AnalysisModelRequest request)
    {
        var model = new AnalysisModel
        {
            Name = request.Name,
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
        var model = await _context.AnalysisModels.FindAsync(id);
        if (model == null)
            throw new DbKeyNotFoundException(id, typeof(AnalysisModel));

        model.Name = request.Name;

        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<Report> CreateReportAsync(CreateReportSubmission submission)
    {
        var modelId = submission.ModelId;
        var model = await GetByIdAsync(modelId);

        if (model == null)
            throw new DbKeyNotFoundException(modelId, typeof(AnalysisModel));

        _logger.LogDebug($"Creating report for model {modelId}: {model.Name}");

        var (result, queriesAndResults) = await _kpiService.EvaluateKPIs(model.KPIs, submission.QueryParameterValues);

        var report = new Report
        {
            Title = submission.Title,
            Notes = submission.Notes,
            KPIsAndValues = result,
            QueryResults = queriesAndResults,
            AnalysisModel = model
        };

        await _context.Reports.AddAsync(report);
        await _context.SaveChangesAsync();

        return report;
    }

    public async Task DeleteReportAsync(Guid reportId)
    {
        var report = await _context.GetByIdOrThrowAsync<Report>(reportId);
        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetRequiredQueriesAsync(Guid modelId)
    {
        var model = await GetByIdAsync(modelId);
        if (model == null)
            throw new DbKeyNotFoundException(modelId, typeof(AnalysisModel));

        var queries = model.KPIs
            .Where(x => x != null && x.Expression != null)
            .SelectMany(x => x.Expression!.GetRequiredQueries())
            .Distinct();
        return queries;
    }
}

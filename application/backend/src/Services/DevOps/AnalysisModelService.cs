using backend.Model.Analysis;
using backend.Model.Analysis.Graphical;
using backend.Model.Analysis.KPIs;
using backend.Model.Analysis.Reports;
using backend.Model.Exceptions;
using backend.Model.Rest;
using backend.Model.Users;
using backend.Services.Database;
using backend.Services.Expressions;
using backend.Services.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Common;

namespace backend.Services.DevOps;

public class AnalysisModelService : IAnalysisModelService
{
    private readonly DataContext m_context;
    private readonly IUserService m_userService;
    private readonly IKPIService m_kpiService;
    private readonly ILogger<AnalysisModelService> m_logger;

    public AnalysisModelService(
        DataContext context, IUserService userService, IKPIService kpiService, ILogger<AnalysisModelService> logger)
    {
        m_context = context;
        m_userService = userService;
        m_kpiService = kpiService;
        m_logger = logger;
    }

    public async Task<AnalysisModel> GetByIdAsync(Guid id)
    {
        var model = await m_context.AnalysisModels
            .Include(x => x.KPIs)
            .Include(x => x.KPIFolders)
                .ThenInclude(x => x.KPIs)
            .Include(x => x.KPIFolders)
                .ThenInclude(x => x.SubFolders)
                    .ThenInclude(x => x.KPIs)
            .Include(x => x.ModelUsers)
                .ThenInclude(x => x.User)
            .Include(x => x.Reports)
            .Include(x => x.Graphical).FirstOrDefaultAsync(x => x.Id == id);

        if (model == null)
            throw new DbKeyNotFoundException(id, typeof(AnalysisModel));

        return model;
    }

    public async Task<List<AnalysisModel>> GetMyModelsAsync()
    {
        var user = await m_userService.GetSelfAsync();
        var models = user.UserModels.Select(x => x.ModelId);
        return await m_context.AnalysisModels
                .Include(x => x.KPIs)
                .Include(x => x.ModelUsers)
                .Include(x => x.Reports)
                .Include(x => x.Graphical)
            .Where(x => models.Contains(x.Id)).ToListAsync();
    }

    public async Task<AnalysisModel> CreateAsync(AnalysisModelRequest request)
    {
        var user = await m_userService.GetSelfAsync();

        var model = new AnalysisModel
        {
            Name = request.Name
        };

        await m_context.AnalysisModels.AddAsync(model);
        await m_context.SaveChangesAsync();

        var userModel = new UserModel
        {
            Model = model,
            User = user,
            Permission = ModelPermission.ADMIN
        };

        await m_context.UserModels.AddAsync(userModel);
        await m_context.SaveChangesAsync();

        return model;
    }

    private async IAsyncEnumerable<KPI> GetAllKPIs(AnalysisModel model)
    {
        foreach (var kpi in model.KPIs)
        {
            yield return kpi;
        }

        foreach (var kpiFolder in model.KPIFolders)
        {
            await foreach (var kpi in GetAllSubKPIs(kpiFolder))
            {
                yield return kpi;
            }
        }
    }

    private async IAsyncEnumerable<KPI> GetAllSubKPIs(KPIFolder folder)
    {
        await m_context.KPIFolders.Entry(folder).Collection(x => x.KPIs).LoadAsync();
        await m_context.KPIFolders.Entry(folder).Collection(x => x.SubFolders).LoadAsync();

        foreach (var kpi in folder.KPIs)
        {
            yield return kpi;
        }

        foreach (var subFolder in folder.SubFolders)
        {
            await foreach (var kpi in GetAllSubKPIs(subFolder))
            {
                if (kpi != null)
                {
                    yield return kpi!;
                }
            }
        }
    }

    public async Task<AnalysisModel> UpdateAsync(Guid id, AnalysisModelUpdate request)
    {
        var model = await GetByIdAsync(id);
        model.Name = request.Name;

        await m_context.SaveChangesAsync();
        return model;
    }

    public async Task<Report> CreateReportAsync(Guid modelId, CreateReportSubmission submission)
    {
        var model = await GetByIdAsync(modelId);
        m_logger.LogDebug($"Creating report for model {modelId}: {model.Name}");

        var kpis = await GetAllKPIs(model).ToListAsync();

        var (result, queriesAndResults) = await m_kpiService.EvaluateKPIs(kpis, submission.QueryParameterValues);

        var report = new Report
        {
            Title = submission.Title,
            Notes = submission.Notes,
            ReportData = new ReportData
            {
                KPIsAndValues = result,
                QueryResults = queriesAndResults,
            },
            AnalysisModel = model
        };

        await m_context.Reports.AddAsync(report);
        await m_context.SaveChangesAsync();

        return report;
    }

    public async Task DeleteReportAsync(Guid reportId)
    {
        var report = await m_context.GetByIdOrThrowAsync<Report>(reportId);
        m_context.Reports.Remove(report);
        await m_context.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> SelectAllRequiredQueries(KPIFolder folder)
    {
        await m_context.Entry(folder).Collection(x => x.KPIs).Query().Include(x => x.Expression).LoadAsync();
        var queries = folder.KPIs
            .Where(x => x != null && x.Expression != null)
            .SelectMany(x => x.Expression!.GetRequiredQueries())
            .ToList();

        foreach (var subFolder in folder.SubFolders)
        {
            var childQueries = await SelectAllRequiredQueries(subFolder);
            queries.AddRange(childQueries);
        }

        return queries;
    }

    public async Task<IEnumerable<string>> GetRequiredQueriesAsync(Guid modelId)
    {
        var model = await GetByIdAsync(modelId);

        await m_context.Entry(model).Collection(x => x.KPIs).Query().Include(x => x.Expression).LoadAsync();
        var queries = model.KPIs
            .Where(x => x != null && x.Expression != null)
            .SelectMany(x => x.Expression!.GetRequiredQueries())
            .Distinct().ToList();

        foreach (var folder in model.KPIFolders)
        {
            var folderQueries = await SelectAllRequiredQueries(folder);
            queries.AddRange(folderQueries);
        }

        return queries.Distinct();
    }

    public async Task<Report> GetReportAsync(Guid id)
    {
        var report = await m_context.GetByIdOrThrowAsync<Report>(id);
        await m_context.Entry(report).Reference(x => x.ReportData).LoadAsync();
        return report;
    }

    public async Task<GraphicalConfiguration> GetGraphicalConfigAsync(Guid id)
    {
        var config = await m_context.GetByIdOrThrowAsync<GraphicalConfiguration>(id);
        await m_context.Entry(config)
            .Collection(x => x.Items)
            .Query()
            .Include(x => x.Layout)
            .Include(x => x.DataSources)
            .Include(x => x.Configuration)
            .Include(x => x.Properties)
            .LoadAsync();

        return config;
    }

    public async Task<GraphicalConfiguration> CreateGraphicalConfigAsync(Guid modelId)
    {
        var model = await m_context.GetByIdOrThrowAsync<AnalysisModel>(modelId);

        await m_context.Entry(model).Collection(x => x.Graphical).Query().Include(x => x.Items).LoadAsync();

        var config = new GraphicalConfiguration { Name = "New graphical configuration", Items = new List<GraphicalReportItem>() };

        model.Graphical.Add(config);

        await m_context.SaveChangesAsync();

        return config;
    }

    public async Task<GraphicalConfiguration> UpdateConfigAsync(Guid id, GraphicalConfigurationUpdate submission)
    {
        if (submission.Name == null)
            throw new BadRequestException("'Name' cannot be null");

        var config = await m_context.GetByIdOrThrowAsync<GraphicalConfiguration>(id);

        config.Name = submission.Name;

        await m_context.SaveChangesAsync();
        return config;
    }

    public async Task DeleteConfigAsync(Guid id)
    {
        var config = await m_context.GetByIdOrThrowAsync<GraphicalConfiguration>(id);
        m_context.Remove(config);
        await m_context.SaveChangesAsync();
    }

    public async Task<GraphicalReportItemLayout> UpdateLayoutAsync(Guid itemId, LayoutUpdateSubmission submission)
    {
        if (submission.Layout == null)
            throw new BadRequestException("Layout cannot be null!");

        var item = await m_context.GetByIdOrThrowAsync<GraphicalReportItem>(itemId);
        await m_context.Entry(item).Reference(x => x.Layout).LoadAsync();

        item.Layout ??= new GraphicalReportItemLayout();

        item.Layout.H = submission.Layout.H;
        item.Layout.W = submission.Layout.W;
        item.Layout.X = submission.Layout.X;
        item.Layout.Y = submission.Layout.Y;

        await m_context.SaveChangesAsync();
        return submission.Layout;
    }

    public async Task<GraphicalReportItem> AddGraphicalConfigLayoutItemAsync(Guid id, AddReportItemSubmission submission)
    {
        var graphicalConfiguration = await m_context.GetByIdOrThrowAsync<GraphicalConfiguration>(id);

        if (submission.Layout == null)
            throw new BadRequestException("Layout cannot be null.");

        if (submission.Layout.MaxH != null && submission.Layout.H > submission.Layout.MaxH)
        {
            throw new BadRequestException("Layout is invalid: h > maxH.");
        }
        if (submission.Layout.MinH != null && submission.Layout.H < submission.Layout.MinH)
        {
            throw new BadRequestException("Layout is invalid: h < minH.");
        }
        if (submission.Layout.MaxW != null && submission.Layout.W > submission.Layout.MaxW)
        {
            throw new BadRequestException("Layout is invalid: w > maxW.");
        }
        if (submission.Layout.MinW != null && submission.Layout.W < submission.Layout.MinW)
        {
            throw new BadRequestException("Layout is invalid: w < minW.");
        }

        var newItem = new GraphicalReportItem
        {
            Name = $"New {submission.Type}",
            Type = submission.Type,
            DataSources = new GraphicalItemDataSources() { KPIs = new List<Guid>() },
            Layout = submission.Layout
        };

        graphicalConfiguration.Items.Add(newItem);
        await m_context.SaveChangesAsync();
        return newItem;
    }
    public async Task DeleteGraphicalConfigLayoutItemAsync(Guid id)
    {
        var item = await m_context.FindAsync<GraphicalReportItem>(id);
        if (item == null)
            return;

        m_context.Remove(item);
        await m_context.SaveChangesAsync();
    }

    public async Task<GraphicalReportItem> UpdateGraphicalConfigLayoutItemAsync(Guid id, UpdateReportItemSubmission submission)
    {
        var item = await m_context.GetByIdOrThrowAsync<GraphicalReportItem>(id);

        item.Name = submission.Name;

        await m_context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateGraphicalConfigItemKPIs(Guid id, UpdateKPIsOfGraphicalItemSubmission submission)
    {
        var item = await m_context.GetByIdOrThrowAsync<GraphicalReportItem>(id);

        await m_context.Entry(item).Reference(x => x.DataSources).LoadAsync();
        item.DataSources ??= new GraphicalItemDataSources();
        item.DataSources.KPIs = submission.KPIs;

        await m_context.SaveChangesAsync();
    }

    public async Task UpdateGraphicalConfigItemProperties(Guid id, UpdatePropertiesOfGraphicalItemSubmission submission)
    {
        var item = await m_context.GetByIdOrThrowAsync<GraphicalReportItem>(id);

        await m_context.Entry(item).Reference(x => x.Properties).LoadAsync();
        item.Properties ??= new GraphicalReportItemProperties();
        item.Properties.ListFields = submission.Properties?.ListFields ?? new List<string>();

        await m_context.SaveChangesAsync();
    }

    public async Task<User?> AddUserToModelAsync(Guid modelId, AddUserToModelSubmission submission)
    {
        var model = await m_context.GetByIdOrThrowAsync<AnalysisModel>(modelId);
        var lowerCaseEmail = submission.Email.ToLowerInvariant();

        var user = await m_context.Users.FirstOrDefaultAsync(x => x.EMail == lowerCaseEmail);

        if (user == null)
        {
            var authenticated = await m_userService.GetSelfAsync();
            var request = new ModelAssociationRequest
            {
                IssuedBy = authenticated,
                Email = submission.Email,
                Model = model
            };

            await m_context.ModelAssociationRequests.AddAsync(request);
            await m_context.SaveChangesAsync();

            m_logger.LogInformation($"Added model assoc request for model '{model.Name}' to user with email '{submission.Email}'.");
            return null;
        }

        var userModel = new UserModel
        {
            Model = model,
            User = user,
            Permission = submission.Permission
        };

        await m_context.UserModels.AddAsync(userModel);
        await m_context.SaveChangesAsync();

        return user;
    }

    public async Task ChangeUserPermissionOnModelAsync(Guid modelId, Guid userId, ModelPermission permission)
    {
        var userModel = await m_context.UserModels.FindAsync(userId, modelId);
        if (userModel == null)
            throw new DbKeyNotFoundException(new { modelId, userId }, typeof(UserModel));

        userModel.Permission = permission;

        await m_context.SaveChangesAsync();
    }
}
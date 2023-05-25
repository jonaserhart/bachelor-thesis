using backend.Model.Analysis;
using backend.Model.Analysis.WorkItems;
using backend.Model.Rest;

namespace backend.Services.DevOps;

public interface IAnalysisModelService
{
    Task<AnalysisModel?> GetByIdAsync(Guid id);
    Task<List<AnalysisModel>> GetMyModelsAsync();
    Task<AnalysisModel> CreateAsync(AnalysisModelRequest request);
    Task<AnalysisModel> UpdateAsync(Guid id, AnalysisModelUpdate request);
    Task<IEnumerable<Project>> GetProjectsAsync();
    Task<IEnumerable<Team>> GetTeamsAsync(string projectId);
    Task<IEnumerable<FieldInfo>> GetFieldInfosAsync(string projectId);
    Task<IEnumerable<Workitem>> GetWorkitemsAsync(string projectId, Iteration iteration, Guid queryId, DateTime? asOf = null);
}
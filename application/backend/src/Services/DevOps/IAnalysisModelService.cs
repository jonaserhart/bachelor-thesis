using backend.Model.Analysis;
using backend.Model.Rest;

namespace backend.Services.DevOps;

public interface IAnalysisModelService
{
    Task<AnalysisModel?> GetByIdAsync(Guid id);
    Task<List<AnalysisModel>> GetMyModelsAsync();
    Task<AnalysisModel> CreateAsync(AnalysisModelRequest request);
    Task<AnalysisModel> UpdateAsync(Guid id,  AnalysisModelUpdate request);
    Task<IEnumerable<Project>> GetProjectsAsync();
}
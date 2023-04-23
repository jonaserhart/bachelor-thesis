using backend.Model.Analysis;
using backend.Model.Users;

namespace backend.Services.API;

public interface IApiClient
{
    Task<User> GetSelfAsync();
    Task<IEnumerable<Project>> GetProjectsAsync(int skip = 0);
    Task<IEnumerable<Team>> GetTeamsAsync(string projectId);
    Task<IEnumerable<Model.Analysis.FieldInfo>> GetWiqlFieldsAsync(Query wiql, string iterationPath);
    Task<IEnumerable<Iteration>> GetIterationsAsync(Guid projectId, Guid teamId);
}
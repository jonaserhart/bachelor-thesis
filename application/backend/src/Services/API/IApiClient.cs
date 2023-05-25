using backend.Model.Analysis;
using backend.Model.Rest;
using backend.Model.Users;

namespace backend.Services.API;

public interface IApiClient : IDisposable
{
    Task<User> GetSelfAsync();
    Task<IEnumerable<Project>> GetProjectsAsync(int skip = 0);
    Task<IEnumerable<Team>> GetTeamsAsync(string projectId);
    Task<IEnumerable<Iteration>> GetIterationsAsync(string projectId, string teamId);
    Task<IEnumerable<Model.Analysis.FieldInfo>> GetFieldInfosAsync(string projectId);
    Task<IEnumerable<QueryResponse>> GetQueriesAsync(string projectId);
    Task<Query> GetQueryByIdAsync(string projectId, string queryId);
}
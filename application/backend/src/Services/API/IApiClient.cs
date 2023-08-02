using backend.Model.Analysis;
using backend.Model.Custom;
using backend.Model.Users;
using Microsoft.TeamFoundation.Core.WebApi.Types;

namespace backend.Services.API;

public interface IApiClient : IDisposable
{
    Task<User> GetSelfAsync();
    Task<IEnumerable<AzureDevopsIteration>> GetIterationsAsync(string projectId, string teamId);
    Task<List<Dictionary<string, object>>> GetIterationTicketsAsync(TeamContext teamContext, Guid iterationId);
    Task<List<Dictionary<string, object>>> GetIterationTasksAsync(TeamContext teamContext, Guid iterationId);
    Task<double> GetIterationCapacities(string project, Guid iteration);
}
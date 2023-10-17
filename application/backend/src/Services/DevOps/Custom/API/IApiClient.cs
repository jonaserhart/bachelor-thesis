using backend.Model.Analysis;
using backend.Model.Custom;
using backend.Model.Users;
using Microsoft.TeamFoundation.Core.WebApi.Types;

namespace backend.Services.DevOps.Custom.API;

public interface IApiClient : IDisposable
{
    Task<IEnumerable<AzureDevopsIteration>> GetIterationsAsync(string projectId, string teamId);
    Task<List<Dictionary<string, object>>> GetIterationTicketsAsync(TeamContext teamContext, Guid iterationId);
    Task<List<Dictionary<string, object>>> GetIterationTasksAsync(TeamContext teamContext, Guid iterationId);
    Task<List<PossibleObjectField>> GetAvailableFields(string project);
    Task<double> GetIterationCapacitiesAsync(TeamContext teamContext, Guid iteration);
    Task<List<Dictionary<string, object>>> GetWorkItemsOfIterationAndAsOfAsync(string project, string iterationPath, DateTime asOf, params string[] workitemTypes);
    Task<List<Dictionary<string, object>>> GetNormalTicketsAsOfAsync(string project, string iterationPath, DateTime asOf);
    Task<List<Dictionary<string, object>>> GetBlockerTicketsAsOfAsync(string project, string iterationPath, DateTime asOf);
    Task<List<Dictionary<string, object>>> GetAfterThoughtTicketsAsOfAsync(string project, string iterationPath, DateTime asOf);
    Task<List<Dictionary<string, object>>> GetRemovedWorkItemsAsync(string project, string iterationPath, DateTime sprintEnd, DateTime sprintStart, params string[] workItemTypes);
}
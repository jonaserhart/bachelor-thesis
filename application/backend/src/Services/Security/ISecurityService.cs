using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace backend.Services.Security;

public interface ISecurityService
{
    Task AuthorizeModelAsync(Guid? modelId, OperationAuthorizationRequirement operation, ClaimsPrincipal user);
}
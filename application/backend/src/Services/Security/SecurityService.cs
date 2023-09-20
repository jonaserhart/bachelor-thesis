using backend.Extensions;
using backend.Model.Exceptions;
using backend.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace backend.Services.Security;

public class SecurityService : ISecurityService
{
    private readonly DataContext _dbContext;
    private readonly IAuthorizationService _auth;

    public SecurityService(DataContext dbContext, IAuthorizationService auth)
    {
        _dbContext = dbContext;
        _auth = auth;
    }

    public async Task AuthorizeModelAsync(Guid? modelId, OperationAuthorizationRequirement operation, ClaimsPrincipal user)
    {
        var project = await _dbContext.AnalysisModels.FindAsync(modelId);
        var authState = await _auth.AuthorizeAsync(user, project, operation);

        if (!authState.Succeeded)
            throw new ForbiddenException(authState.FailureReasonsToString());
    }
}
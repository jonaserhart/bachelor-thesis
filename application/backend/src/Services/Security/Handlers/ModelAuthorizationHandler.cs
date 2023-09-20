using backend.Model.Analysis;
using backend.Model.Security;
using backend.Model.Users;
using backend.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Services.Security.Handlers;

public class ModelAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, AnalysisModel>
{
    private readonly DataContext m_context;

    public ModelAuthorizationHandler(DataContext context)
    {
        m_context = context;
    }

    public static bool CanView(ModelPermission permission)
        => permission == ModelPermission.EDITOR || permission == ModelPermission.READER || permission == ModelPermission.ADMIN;
    public static bool CanEditProject(ModelPermission permission)
        => permission == ModelPermission.EDITOR || permission == ModelPermission.ADMIN;
    public static bool CanDeleteProject(ModelPermission permission)
        => permission == ModelPermission.ADMIN;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, AnalysisModel resource)
    {
        if (resource == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "No resource provided"));
            return;
        }

        var userId = context
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)
            || !Guid.TryParse(userId, out var id))
        {
            context.Fail(new AuthorizationFailureReason(this, "No User found in auth context"));
            return;
        }

        var user = await m_context.Users.Include(x => x.UserModels).FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            context.Fail(new AuthorizationFailureReason(this, $"No User found for authenticated id {id}"));
            return;
        }

        var userModel = user.UserModels.FirstOrDefault(x => x.ModelId == resource.Id);

        if (userModel == null)
        {
            context.Fail(new AuthorizationFailureReason(this, $"No User {id} is not associated with model {resource.Name}"));
            return;
        }

        switch (requirement.Name)
        {
            case Operations.EditModelOperation:
            case Operations.EditGraphicalConfigOperation:
            case Operations.EditGraphicalItemKPIsOperation:
            case Operations.EditGraphicalItemLayoutOperation:
            case Operations.EditGraphicalItemOperation:
            case Operations.EditGraphicalItemPropertiesOperation:
            case Operations.EditKPIFolderOperation:
            case Operations.EditKPIOperation:
            case Operations.CreateGraphicalConfigOperation:
            case Operations.CreateGraphicalItemKPIsOperation:
            case Operations.CreateKPIFolderOperation:
            case Operations.CreateKPIOperation:
            case Operations.CreateReportOperation:
                if (!CanEditProject(userModel.Permission))
                    context.Fail(new AuthorizationFailureReason(this, $"User {user.Id} ({user.DisplayName}) has insufficient permissions to edit {resource.Name}"));
                break;

            case Operations.ViewModelOperation:
            case Operations.ViewGraphicalConfigOperation:
            case Operations.ViewReportOperation:
            case Operations.ViewKPIOperation:
                if (!CanView(userModel.Permission))
                    context.Fail(new AuthorizationFailureReason(this, $"User {user.Id} ({user.DisplayName}) has insufficient permissions to view {resource.Name}"));
                break;
            case Operations.DeleteModelOperation:
            case Operations.DeleteGraphicalConfigOperation:
            case Operations.DeleteGraphicalItemOperation:
            case Operations.DeleteKPIFolderOperation:
            case Operations.DeleteKPIOperation:
            case Operations.DeleteReportOperation:
            case Operations.AddUserToModelOperation:
            case Operations.ChangeUserRoleOnModelOperation:
                if (!CanDeleteProject(userModel.Permission))
                    context.Fail(new AuthorizationFailureReason(this, $"User {user.Id} ({user.DisplayName}) has insufficient permissions to delete {resource.Name}"));
                break;
            default:
                break;
        }

        if (context.HasFailed)
            return;

        context.Succeed(requirement);
    }
}
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace backend.Model.Security;

public class Operations
{
    public const string ViewModelOperation = "ViewModel";
    public const string EditModelOperation = "EditModel";
    public const string DeleteModelOperation = "DeleteModel";
    public const string CreateGraphicalConfigOperation = "CreateGraphicalConfig";
    public const string ViewGraphicalConfigOperation = "ViewGraphicalConfigOperation";
    public const string EditGraphicalConfigOperation = "EditGraphicalConfigOperation";
    public const string DeleteGraphicalConfigOperation = "DeleteGraphicalConfigOperation";
    public const string CreateGraphicalItemKPIsOperation = "CreateGraphicalItemKPIsOperation";
    public const string DeleteGraphicalItemOperation = "DeleteGraphicalItemOperation";
    public const string EditGraphicalItemOperation = "EditGraphicalItemOperation";
    public const string EditGraphicalItemKPIsOperation = "EditGraphicalItemKPIsOperation";
    public const string EditGraphicalItemPropertiesOperation = "EditGraphicalItemPropertiesOperation";
    public const string EditGraphicalItemLayoutOperation = "EditGraphicalItemLayoutOperation";
    public const string CreateReportOperation = "CreateReportOperation";
    public const string DeleteReportOperation = "DeleteReportOperation";
    public const string ViewReportOperation = "ViewReportOperation";
    public const string AddUserToModelOperation = "AddUserToModelOperation";
    public const string ChangeUserRoleOnModelOperation = "ChangeUserRoleOnModelOperation";
    public const string CreateKPIFolderOperation = "CreateKPIFolderOperation";
    public const string EditKPIFolderOperation = "EditKPIFolderOperation";
    public const string DeleteKPIFolderOperation = "DeleteKPIFolderOperation";
    public const string CreateKPIOperation = "CreateKPIOperation";
    public const string ViewKPIOperation = "ViewKPIOperation";
    public const string EditKPIOperation = "EditKPIOperation";
    public const string DeleteKPIOperation = "DeleteKPIOperation";

    public static OperationAuthorizationRequirement ViewModel => new() { Name = ViewModelOperation };
    public static OperationAuthorizationRequirement EditModel => new() { Name = EditModelOperation };
    public static OperationAuthorizationRequirement DeleteModel => new() { Name = DeleteModelOperation };
    public static OperationAuthorizationRequirement CreateGraphicalConfig => new() { Name = CreateGraphicalConfigOperation };
    public static OperationAuthorizationRequirement ViewGraphicalConfig => new() { Name = ViewGraphicalConfigOperation };
    public static OperationAuthorizationRequirement EditGraphicalConfig => new() { Name = EditGraphicalConfigOperation };
    public static OperationAuthorizationRequirement DeleteGraphicalConfig => new() { Name = DeleteGraphicalConfigOperation };
    public static OperationAuthorizationRequirement CreateGraphicalItemKPIs => new() { Name = CreateGraphicalItemKPIsOperation };
    public static OperationAuthorizationRequirement DeleteGraphicalItem => new() { Name = DeleteGraphicalItemOperation };
    public static OperationAuthorizationRequirement EditGraphicalItem => new() { Name = EditGraphicalItemOperation };
    public static OperationAuthorizationRequirement EditGraphicalItemKPIs => new() { Name = EditGraphicalItemKPIsOperation };
    public static OperationAuthorizationRequirement EditGraphicalItemProperties => new() { Name = EditGraphicalItemPropertiesOperation };
    public static OperationAuthorizationRequirement EditGraphicalItemLayout => new() { Name = EditGraphicalItemLayoutOperation };
    public static OperationAuthorizationRequirement CreateReport => new() { Name = CreateReportOperation };
    public static OperationAuthorizationRequirement DeleteReport => new() { Name = DeleteReportOperation };
    public static OperationAuthorizationRequirement ViewReport => new() { Name = ViewReportOperation };
    public static OperationAuthorizationRequirement AddUserToModel => new() { Name = AddUserToModelOperation };
    public static OperationAuthorizationRequirement ChangeUserRoleOnModel => new() { Name = ChangeUserRoleOnModelOperation };
    public static OperationAuthorizationRequirement CreateKPIFolder => new() { Name = CreateKPIFolderOperation };
    public static OperationAuthorizationRequirement EditKPIFolder => new() { Name = EditKPIFolderOperation };
    public static OperationAuthorizationRequirement DeleteKPIFolder => new() { Name = DeleteKPIFolderOperation };
    public static OperationAuthorizationRequirement CreateKPI => new() { Name = CreateKPIOperation };
    public static OperationAuthorizationRequirement ViewKPI => new() { Name = ViewKPIOperation };
    public static OperationAuthorizationRequirement EditKPI => new() { Name = EditKPIOperation };
    public static OperationAuthorizationRequirement DeleteKPI => new() { Name = DeleteKPIOperation };
}

using System.Security.Claims;
using backend.Model.Exceptions;
using backend.Model.Users;
using backend.Services.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Common;

namespace backend.Services.Users;

public class UserService : IUserService
{
    private readonly DataContext m_context;
    private readonly ILogger<UserService> m_logger;
    private readonly IHttpContextAccessor m_httpContextAccessor;

    public UserService(DataContext context, ILogger<UserService> logger, IHttpContextAccessor contextAccessor)
    {
        m_context = context;
        m_logger = logger;
        m_httpContextAccessor = contextAccessor;
    }

    /// <summary>
    /// Method to create and update a user (user is created if id is not found)
    /// </summary>
    /// <param name="user">User to update or create</param>
    /// <returns>Created or updated user</returns>
    public async Task<User> CreateOrUpdateUserAsync(User user)
    {
        var existing = await m_context.Users.FindAsync(user.Id);

        if (existing != null)
        {
            m_logger.LogDebug($"Updating user {user.Id}.");
            existing.DisplayName = user.DisplayName;
            existing.EMail = user.EMail.ToLowerInvariant();
        }
        else
        {
            user.EMail = user.EMail.ToLowerInvariant();
            m_logger.LogDebug($"Creating user {user.Id}.");
            existing = (await m_context.Users.AddAsync(user)).Entity;

            var modelAssocRequests = await m_context.ModelAssociationRequests.Where(x => x.Email == user.EMail && !x.Completed).ToListAsync();
            foreach (var request in modelAssocRequests)
            {
                var model = await m_context.AnalysisModels.FindAsync(request.ModelId);
                var userModel = new UserModel
                {
                    Model = model,
                    User = existing,
                    Permission = request.Permission,
                };

                await m_context.UserModels.AddAsync(userModel);

                request.Completed = true;
                request.CompletedAt = DateTime.Now.ToUnixEpochTime();
            }
        }

        await m_context.SaveChangesAsync();
        return existing;
    }

    /// <summary>
    /// Method to get a User by Id
    /// </summary>
    /// <param name="userId">UserId to search for</param>
    /// <returns>A User if the Id was found, null otherwise</returns>
    public async Task<User> GetByIdAsync(Guid userId)
    {
        var user = await m_context.GetByIdOrThrowAsync<User>(userId);
        await m_context.Entry(user).Collection(x => x.UserModels).LoadAsync();
        return user;
    }

    /// <summary>
    /// Method to delete a User from the database
    /// </summary>
    /// <param name="userId">Id of the user to delete</param>
    /// <exception cref="DbKeyNotFoundException" />
    /// <returns></returns>
    public async Task DeleteUserAsync(Guid userId)
    {
        var found = await GetByIdAsync(userId);

        m_context.Users.Remove(found);

        await m_context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the currently authenticated user via HttpContext
    /// </summary>
    /// <exception cref="UnauthorizedException" />
    /// <returns>The currently authenticated user</returns>
    public async Task<User> GetSelfAsync()
    {
        var userId = m_httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)
            || !Guid.TryParse(userId, out var id))
            throw new UnauthorizedException("Could not find nameidentifier claim in token.");

        var user = await GetByIdAsync(id);

        return user;
    }
}
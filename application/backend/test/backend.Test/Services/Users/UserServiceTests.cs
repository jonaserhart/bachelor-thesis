using backend.Model.Exceptions;
using backend.Model.Users;
using backend.Services.Users;
using backend.Test.Helpers;
using Moq;

namespace backend.Test.Services.Users;

[TestFixture]
public class UserServiceTests
{

    #region CreateOrUpdateUserAsync

    [Test]
    public async Task CreateOrUpdateUserAsync_GIVE_NonExistentUser_SAVES_NewUser()
    {
        using var dbContext = await TestServices.GetDatabaseContextAsync();
        var logger = TestServices.GetMockedLogger<UserService>();
        var userService = new UserService(dbContext, logger, null);
        var userToCreate = new User()
        {
            EMail = "email@user.com",
            DisplayName = "userDisplayName"
        };

        await userService.CreateOrUpdateUserAsync(userToCreate);

        var inDb = dbContext.Users.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(inDb, Is.Not.Null);
            Assert.That(inDb.Count, Is.EqualTo(1));
            var userInDb = inDb.FirstOrDefault();
            Assert.That(userInDb, Is.Not.Null);
            Assert.That(userInDb?.DisplayName, Is.EqualTo(userToCreate.DisplayName));
            Assert.That(userInDb?.EMail, Is.EqualTo(userToCreate.EMail));
        });
    }

    [Test]
    public async Task CreateOrUpdateUserAsync_GIVE_ExistingUser_UPDATED_UserInDatabase()
    {
        using var dbContext = await TestServices.GetDatabaseContextAsync();
        var logger = TestServices.GetMockedLogger<UserService>();
        var userService = new UserService(dbContext, logger, null);
        var existingUser = new User()
        {
            EMail = "email@user.com",
            DisplayName = "userDisplayName"
        };

        await dbContext.AddAllToDatabase(existingUser);

        // Update user properties
        var updatedDisplayName = "new display name";
        var updatedEmail = "new@email.com";
        existingUser.DisplayName = updatedDisplayName;
        existingUser.EMail = updatedEmail;

        await userService.CreateOrUpdateUserAsync(existingUser);

        var inDb = dbContext.Users.FirstOrDefault();
        Assert.Multiple(() =>
        {
            Assert.That(inDb, Is.Not.Null);
            Assert.That(inDb?.DisplayName, Is.EqualTo(updatedDisplayName));
            Assert.That(inDb?.EMail, Is.EqualTo(updatedEmail));
        });
    }

    #endregion

    #region GetByIdAsync

    [Test]
    public async Task GetByIdAsync_GIVE_NonExistentUserId_RETURNS_Null()
    {
        using var context = await TestServices.GetDatabaseContextAsync();

        var userService = new UserService(context, null, null);

        var found = await userService.GetByIdAsync(Guid.NewGuid());

        Assert.That(found, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_GIVE_ExistentUserId_RETURNS_User()
    {
        using var context = await TestServices.GetDatabaseContextAsync();

        var userService = new UserService(context, null, null);

        var userId = Guid.NewGuid();
        var userInDb = new User
        {
            Id = userId
        };
        await context.AddAllToDatabase(userInDb);

        var found = await userService.GetByIdAsync(userId);

        Assert.That(found, Is.InstanceOf<User>());
    }

    #endregion

    #region DeleteUserAsync

    [Test]
    public async Task DeleteUserAsync_GIVE_ExistingId_DELETES_UserFromDb()
    {
        using var context = await TestServices.GetDatabaseContextAsync();

        var userService = new UserService(context, null, null);

        var userIdToDelete = Guid.NewGuid();
        var userInDb = new User
        {
            Id = userIdToDelete
        };

        await context.AddAllToDatabase(userInDb);

        var inDb = await context.Users.FindAsync(userIdToDelete);
        if (inDb == null)
        {
            Assert.Fail("User is not in database => cannot be deleted.");
        }

        await userService.DeleteUserAsync(userIdToDelete);

        var firstInDb = context.Users.FirstOrDefault();

        // Assert that no users are in database after deleting the only existent user
        Assert.That(firstInDb, Is.Null);
    }

    [Test]
    public async Task DeleteUserAsync_GIVE_NonExistingId_THROWS_DbKeyNotFoundException()
    {
        using var context = await TestServices.GetDatabaseContextAsync();

        var userService = new UserService(context, null, null);

        var userIdToDelete = Guid.NewGuid();

        var asyncThunk = async () =>
            await userService.DeleteUserAsync(userIdToDelete);
        ;

        Assert.That(asyncThunk, Throws.InstanceOf<DbKeyNotFoundException>());
    }

    #endregion

    #region GetSelfAsync
    [Test]
    public void GetSelfAsync_WITH_NoUserInHttpContext_THROWS_UnauthorizedException()
    {
        var contextAccessor = TestServices.GetMockedHttpContextAccessor();

        var userService = new UserService(null, null, contextAccessor);

        var getSelfMethod = async () => await userService.GetSelfAsync();

        Assert.That(getSelfMethod, Throws.TypeOf<UnauthorizedException>().And.Message.Contains("Could not find nameidentifier claim in token"));
    }

    [Test]
    public async Task GetSelfAsync_WITH_NonExistentUserInHttpContext_THROWS_UnauthorizedException()
    {
        using var dbContext = await TestServices.GetDatabaseContextAsync();

        // User Id that claims to be authenticated but is not registered in database
        var userIdInContext = Guid.NewGuid();

        var contextAccessor = TestServices.GetMockedHttpContextAccessor(authenticatedUserid: userIdInContext.ToString());
        var userService = new UserService(dbContext, null, contextAccessor);

        var getSelfMethod = async () => await userService.GetSelfAsync();

        Assert.That(getSelfMethod, Throws.TypeOf<UnauthorizedException>().And.Message.Contains($"Could not find User with Id {userIdInContext} in database"));
    }

    [Test]
    public async Task GetSelfAsync_WITH_ExistentUserInHttpContext_RETURNS_User()
    {
        using var dbContext = await TestServices.GetDatabaseContextAsync();

        // User Id that claims to be authenticated but is not registered in database
        var userIdInContext = Guid.NewGuid();
        var user = new User
        {
            Id = userIdInContext
        };

        await dbContext.AddAllToDatabase(user);

        var contextAccessor = TestServices.GetMockedHttpContextAccessor(authenticatedUserid: userIdInContext.ToString());
        var userService = new UserService(dbContext, null, contextAccessor);

        var authenticatedUser = await userService.GetSelfAsync();

        Assert.That(authenticatedUser.Id, Is.EqualTo(user.Id));
    }
    #endregion
}
using System.Security.Claims;
using backend.Services.Database;
using backend.Services.DevOps.Custom.API;
using backend.Test.Helpers.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.Test.Helpers;

public static class TestServices
{
    /// <summary>
    /// Gets a in-memory database for testing
    /// <br/>
    /// example: <code>using var dbContext = await TestServices.GetDatabaseContext;</code>
    /// </summary>
    /// <returns>A DataContext object for testing</returns>
    public static async Task<DataContext> GetDatabaseContextAsync()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new DataContext(options);
        await databaseContext.Database.EnsureCreatedAsync();
        return databaseContext;
    }

    /// <summary>
    /// Gets a in-memory database for testing
    /// <br/>
    /// example: <code>using var dbContext = TestServices.GetDatabaseContext;</code>
    /// </summary>
    /// <returns>A DataContext object for testing</returns>
    public static DataContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var databaseContext = new DataContext(options);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }

    public static async Task<T[]> AddAllToDatabase<T>(this DataContext database, params T[] objs)
    {
        try
        {
            foreach (var obj in objs.Where(x => x != null))
                await database.AddAsync(obj!);
            await database.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new TestSetupException($"Could not add objects to database: ${objs.Select(x => x?.ToString())}", e);
        }

        return objs;
    }

    public static ILogger<T> GetMockedLogger<T>()
    {
        var mock = new Mock<ILogger<T>>();
        return mock.Object;
    }

    public static IHttpContextAccessor GetMockedHttpContextAccessor(bool withMockedHttpContext = false, string? authenticatedUserid = null)
    {
        var mock = new Mock<IHttpContextAccessor>();
        var shouldMockHttpContext = withMockedHttpContext || !string.IsNullOrEmpty(authenticatedUserid);
        if (shouldMockHttpContext)
        {
            var httpContextMock = new Mock<HttpContext>();

            if (!string.IsNullOrEmpty(authenticatedUserid))
            {
                var claimsPrincipal = new Mock<ClaimsPrincipal>();
                claimsPrincipal.Setup(x => x.FindFirst(It.Is<string>(x => x == ClaimTypes.NameIdentifier)))
                    .Returns(new Claim(ClaimTypes.NameIdentifier, authenticatedUserid));

                httpContextMock.SetupGet(x => x.User).Returns(claimsPrincipal.Object);
            }

            mock.SetupGet(x => x.HttpContext)
                .Returns(httpContextMock.Object);
        }
        return mock.Object;
    }

    public static IApiClientFactory GetMockedApiClientFactory()
    {
        var mock = new Mock<IApiClientFactory>();
        return mock.Object;
    }

}
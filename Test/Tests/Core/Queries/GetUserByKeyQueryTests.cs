using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Queries;

[TestClass]
public class GetUserByKeyQueryTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly GetUserByKeyQueryHandler underTest;

    public GetUserByKeyQueryTests()
    {
        testUserRepository = new();
        underTest = new(testUserRepository);
    }

    private GetUserByKeyQuery BuildQuery(UserStatus authenticatedUserStatus = UserStatus.Admin, Guid? requestedUserKey = null)
    {
        var authenticatedUser = RegisteredUser.Default with
        {
            FirstName = "Auth",
            LastName = "User",
            Status = authenticatedUserStatus
        };
        testUserRepository.AddEntryForTest(authenticatedUser);

        var targetUserKey = requestedUserKey ?? authenticatedUser.Key;

        // Add the requested user if different from authenticated user
        if (requestedUserKey.HasValue && requestedUserKey != authenticatedUser.Key)
        {
            var requestedUser = RegisteredUser.Default with
            {
                Key = requestedUserKey.Value,
                FirstName = "Requested",
                LastName = "User"
            };
            testUserRepository.AddEntryForTest(requestedUser);
        }

        return new(authenticatedUser.Key, targetUserKey);
    }

    [TestMethod]
    public async Task QueryReturnsRequestedUser()
    {
        var requestedUserKey = Guid.NewGuid();
        var query = BuildQuery(UserStatus.Admin, requestedUserKey);

        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(requestedUserKey, result.Key);
        Assert.AreEqual("Requested", result.FirstName);
    }

    [TestMethod]
    public async Task UserCanAccessTheirOwnProfile()
    {
        var query = BuildQuery(UserStatus.Approved);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task AdminCanAccessAnyUserProfile()
    {
        var otherUserKey = Guid.NewGuid();
        var query = BuildQuery(UserStatus.Admin, otherUserKey);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task ApprovedUserCannotAccessOtherUserProfile()
    {
        var otherUserKey = Guid.NewGuid();
        var query = BuildQuery(UserStatus.Approved, otherUserKey);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task UnapprovedUserCanAccessTheirOwnProfile()
    {
        var query = BuildQuery(UserStatus.Unapproved);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task UnapprovedUserCannotAccessOtherUserProfile()
    {
        var otherUserKey = Guid.NewGuid();
        var query = BuildQuery(UserStatus.Unapproved, otherUserKey);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }
}

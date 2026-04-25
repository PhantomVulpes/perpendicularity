using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Queries;

[TestClass]
public class GetAllUsersQueryTests
{
    private readonly TestQueryProvider<RegisteredUser> testQueryProvider;
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly GetAllUsersQueryHandler underTest;

    public GetAllUsersQueryTests()
    {
        testQueryProvider = new();
        testUserRepository = new();
        underTest = new(testQueryProvider, testUserRepository);
    }

    private GetAllUsersQuery BuildQuery(UserStatus authenticatedUserStatus = UserStatus.Admin)
    {
        var authenticatedUser = RegisteredUser.Default with
        {
            Status = authenticatedUserStatus
        };
        testUserRepository.AddEntryForTest(authenticatedUser);

        return new(authenticatedUser.Key);
    }

    [TestMethod]
    public async Task QueryReturnsEmptyResultWhenNoUsersExist()
    {
        var query = BuildQuery();

        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task QueryReturnsAllUsersFromProvider()
    {
        var user1 = RegisteredUser.Default with { FirstName = "Alice" };
        var user2 = RegisteredUser.Default with { FirstName = "Bob" };
        var user3 = RegisteredUser.Default with { FirstName = "Charlie" };

        testQueryProvider.Response.AddRange([user1, user2, user3]);

        var query = BuildQuery();
        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
        CollectionAssert.Contains(result.ToList(), user1);
        CollectionAssert.Contains(result.ToList(), user2);
        CollectionAssert.Contains(result.ToList(), user3);
    }

    [TestMethod]
    public async Task AdminsHaveAccess()
    {
        var query = BuildQuery(UserStatus.Admin);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task ApprovedUsersDoNotHaveAccess()
    {
        var query = BuildQuery(UserStatus.Approved);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task UnapprovedUsersDoNotHaveAccess()
    {
        var query = BuildQuery(UserStatus.Unapproved);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }
}

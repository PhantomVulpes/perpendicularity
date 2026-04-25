using System.Security.Authentication;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;
using Vulpes.Perpendicularity.Core.ValueObjects;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Security;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Queries;

[TestClass]
public class GetUserByLoginCredentialsQueryTests
{
    private readonly TestQueryProvider<RegisteredUser> testQueryProvider;
    private readonly TestKnoxHasher testKnoxHasher;
    private readonly GetUserByLoginCredentialsQueryHandler underTest;

    public GetUserByLoginCredentialsQueryTests()
    {
        testQueryProvider = new();
        testKnoxHasher = new();
        underTest = new(testQueryProvider, testKnoxHasher);
    }

    private static GetUserByLoginCredentialsQuery BuildQuery(string firstName = "John", string lastName = "Doe", string password = "correct-password")
        => new(firstName, lastName, password);

    private static RegisteredUser BuildUser(string firstName = "John", string lastName = "Doe", string passwordHash = "hashed-password")
    {
        return RegisteredUser.Default with
        {
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = HashedString.Empty with { Value = passwordHash }
        };
    }

    [TestMethod]
    public async Task ValidCredentialsReturnUser()
    {
        var user = BuildUser("John", "Doe", "hashed-password");
        testQueryProvider.Response.Add(user);
        testKnoxHasher.HashResults.Add(("correct-password", true));

        var query = BuildQuery("John", "Doe", "correct-password");
        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Key, result.Key);
        Assert.AreEqual("John", result.FirstName);
        Assert.AreEqual("Doe", result.LastName);
    }

    [TestMethod]
    public async Task FirstNameIsCaseInsensitive()
    {
        var user = BuildUser("John", "Doe", "hashed-password");
        testQueryProvider.Response.Add(user);
        testKnoxHasher.HashResults.Add(("correct-password", true));

        var query = BuildQuery("jOhN", "Doe", "correct-password");
        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Key, result.Key);
    }

    [TestMethod]
    public async Task LastNameIsCaseInsensitive()
    {
        var user = BuildUser("John", "Doe", "hashed-password");
        testQueryProvider.Response.Add(user);
        testKnoxHasher.HashResults.Add(("correct-password", true));

        var query = BuildQuery("John", "dOe", "correct-password");
        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Key, result.Key);
    }

    [TestMethod]
    public async Task InvalidFirstNameThrowsException()
    {
        var user = BuildUser("John", "Doe", "hashed-password");
        testQueryProvider.Response.Add(user);

        var query = BuildQuery("Jane", "Doe", "correct-password");

        await Assert.ThrowsExceptionAsync<InvalidCredentialException>(
            async () => await underTest.RequestAsync(query)
        );
    }

    [TestMethod]
    public async Task InvalidLastNameThrowsException()
    {
        var user = BuildUser("John", "Doe", "hashed-password");
        testQueryProvider.Response.Add(user);

        var query = BuildQuery("John", "Smith", "correct-password");

        await Assert.ThrowsExceptionAsync<InvalidCredentialException>(
            async () => await underTest.RequestAsync(query)
        );
    }

    [TestMethod]
    public async Task InvalidPasswordThrowsException()
    {
        var user = BuildUser("John", "Doe", "hashed-password");
        testQueryProvider.Response.Add(user);
        testKnoxHasher.HashResults.Add(("wrong-password", false));

        var query = BuildQuery("John", "Doe", "wrong-password");

        await Assert.ThrowsExceptionAsync<InvalidCredentialException>(
            async () => await underTest.RequestAsync(query)
        );
    }

    [TestMethod]
    public async Task NoMatchingUserThrowsException()
    {
        var user = BuildUser("Jane", "Smith", "hashed-password");
        testQueryProvider.Response.Add(user);

        var query = BuildQuery("John", "Doe", "correct-password");

        await Assert.ThrowsExceptionAsync<InvalidCredentialException>(
            async () => await underTest.RequestAsync(query)
        );
    }

    [TestMethod]
    public async Task MultipleUsersCanExistButOnlyMatchingOneReturned()
    {
        var user1 = BuildUser("Jane", "Smith", "hashed-password-1");
        var user2 = BuildUser("John", "Doe", "hashed-password-2");
        var user3 = BuildUser("Bob", "Johnson", "hashed-password-3");

        testQueryProvider.Response.AddRange([user1, user2, user3]);
        testKnoxHasher.HashResults.Add(("correct-password", true));

        var query = BuildQuery("John", "Doe", "correct-password");
        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(user2.Key, result.Key);
        Assert.AreEqual("John", result.FirstName);
        Assert.AreEqual("Doe", result.LastName);
    }

    [TestMethod]
    public async Task AllUsersHaveAccess()
    {
        var query = BuildQuery();

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }
}

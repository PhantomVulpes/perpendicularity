using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Security;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class RegisterNewUserCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestKnoxHasher testKnoxHasher;

    private readonly RegisterNewUserCommandHandler underTest;

    public RegisterNewUserCommandTests()
    {
        testUserRepository = new();
        testKnoxHasher = new();

        underTest = new(testUserRepository, testKnoxHasher);
    }

    private static RegisterNewUserCommand BuildCommand() => new(Guid.NewGuid(), "Peanut", "Nuggins", "password123");

    [TestMethod]
    public async Task UserIsAddedToRepository()
    {
        testKnoxHasher.MockedHashResult = "hashed-password";
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testUserRepository.InsertedEntries.Count);
    }

    [TestMethod]
    public async Task UserIsUnapprovedByDefault()
    {
        testKnoxHasher.MockedHashResult = "hashed-password";
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.InsertedEntries.First();

        Assert.AreEqual(UserStatus.Unapproved, result.Status);
    }

    [TestMethod]
    public async Task PasswordIsHashed()
    {
        testKnoxHasher.MockedHashResult = "hashed-password-result";
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.InsertedEntries.First();

        Assert.AreEqual("hashed-password-result", result.PasswordHash.Value);
    }

    [TestMethod]
    public async Task UserDetailsAreSet()
    {
        testKnoxHasher.MockedHashResult = "hashed-password";
        var command = new RegisterNewUserCommand(Guid.NewGuid(), "Jane", "Smith", "password456");

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.InsertedEntries.First();

        Assert.AreEqual("Jane", result.FirstName);
        Assert.AreEqual("Smith", result.LastName);
    }

    [TestMethod]
    public async Task AnyoneCanRegister()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand());

        Assert.IsTrue(access);
    }
}

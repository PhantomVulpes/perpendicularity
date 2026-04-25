using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class AddExternalProjectCommandTests
{
    private readonly TestModelRepository<ExternalProject> testProjectRepository;
    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    private readonly AddExternalProjectCommandHandler underTest;

    public AddExternalProjectCommandTests()
    {
        testProjectRepository = new();
        testUserRepository = new();

        underTest = new(testProjectRepository, testUserRepository);
    }

    private AddExternalProjectCommand BuildCommand(UserStatus userStatus = UserStatus.Admin)
    {
        var user = RegisteredUser.Default with
        {
            Status = userStatus
        };

        testUserRepository.AddEntryForTest(user);

        return new(Guid.NewGuid(), "my favorite fruits", "www.fruitytootyinmybooty.com", "all about them fruits", user.Key);
    }

    [TestMethod]
    public async Task ProjectIsAddedToRepository()
    {
        await underTest.ExecuteAsync(BuildCommand());

        Assert.AreEqual(1, testProjectRepository.InsertedEntries.Count);
    }

    [TestMethod]
    public async Task NonAdminsDoNotHaveAccess()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(UserStatus.Approved));

        Assert.IsFalse(access);
    }
}
using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class DeleteExternalProjectCommandTests
{
    private readonly TestModelRepository<ExternalProject> testProjectRepository;
    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    private readonly DeleteExternalProjectCommandHandler underTest;

    public DeleteExternalProjectCommandTests()
    {
        testProjectRepository = new();
        testUserRepository = new();

        underTest = new(testProjectRepository, testUserRepository);
    }

    private DeleteExternalProjectCommand BuildCommand(UserStatus userStatus = UserStatus.Admin)
    {
        var user = RegisteredUser.Default with
        {
            Status = userStatus
        };

        var project = ExternalProject.Default;

        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        return new(project.Key, user.Key);
    }

    [TestMethod]
    public async Task ProjectIsDeleted()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.DeletedEntries.Count);
    }

    [TestMethod]
    public async Task NonAdminsDoNotHaveAccess()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(UserStatus.Approved));

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task AdminsHaveAccess()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(UserStatus.Admin));

        Assert.IsTrue(access);
    }
}

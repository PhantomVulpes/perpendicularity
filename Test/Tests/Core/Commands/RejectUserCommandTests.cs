using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class RejectUserCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    private readonly RejectUserCommandHandler underTest;

    public RejectUserCommandTests()
    {
        testUserRepository = new();

        underTest = new(testUserRepository);
    }

    private RejectUserCommand BuildCommand(UserStatus authenticatedUserStatus = UserStatus.Admin, UserStatus requestedUserStatus = UserStatus.Unapproved)
    {
        var authenticatedUser = RegisteredUser.Default with
        {
            Status = authenticatedUserStatus
        };

        var requestedUser = RegisteredUser.Default with
        {
            Status = requestedUserStatus
        };

        testUserRepository.AddEntryForTest(authenticatedUser);
        testUserRepository.AddEntryForTest(requestedUser);

        return new(authenticatedUser.Key, requestedUser.Key);
    }

    [TestMethod]
    public async Task UserGetsRejected()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.AreEqual(UserStatus.Rejected, result.Status);
    }

    [TestMethod]
    public async Task NonAdminsDoNotHaveAccess()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(authenticatedUserStatus: UserStatus.Approved));

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task CannotRejectNonUnapprovedUser()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(requestedUserStatus: UserStatus.Approved));

        Assert.IsFalse(access);
    }
}

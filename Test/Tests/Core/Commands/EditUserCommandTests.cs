using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Security;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class EditUserCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestKnoxHasher testKnoxHasher;

    private readonly EditUserCommandHandler underTest;

    public EditUserCommandTests()
    {
        testUserRepository = new();
        testKnoxHasher = new();

        underTest = new(testUserRepository, testKnoxHasher);
    }

    private EditUserCommand BuildCommand(UserStatus authorizedUserStatus = UserStatus.Approved, UserStatus userToEditStatus = UserStatus.Approved,
        bool isEditingSelf = true, string? firstName = null, string? lastName = null, string? passwordRaw = null, UserStatus? status = null)
    {
        var authorizedUser = RegisteredUser.Default with
        {
            Status = authorizedUserStatus
        };

        var userToEdit = isEditingSelf
            ? authorizedUser
            : RegisteredUser.Default with
            {
                Status = userToEditStatus
            };

        testUserRepository.AddEntryForTest(authorizedUser);
        if (!isEditingSelf)
        {
            testUserRepository.AddEntryForTest(userToEdit);
        }

        return new(userToEdit.Key, authorizedUser.Key, firstName, lastName, passwordRaw, status);
    }

    [TestMethod]
    public async Task FirstNameGetsUpdated()
    {
        var command = BuildCommand(firstName: "NewFirstName");

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.AreEqual("NewFirstName", result.FirstName);
    }

    [TestMethod]
    public async Task LastNameGetsUpdated()
    {
        var command = BuildCommand(lastName: "NewLastName");

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.AreEqual("NewLastName", result.LastName);
    }

    [TestMethod]
    public async Task PasswordGetsUpdated()
    {
        testKnoxHasher.MockedHashResult = "hashed-password-result";
        var command = BuildCommand(passwordRaw: "newPassword123");

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.AreEqual("hashed-password-result", result.PasswordHash.Value);
    }

    [TestMethod]
    public async Task StatusGetsUpdated()
    {
        var command = BuildCommand(authorizedUserStatus: UserStatus.Admin, status: UserStatus.Inactive);

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.AreEqual(UserStatus.Inactive, result.Status);
    }

    [TestMethod]
    public async Task RejectedUserBecomesUnapprovedWhenEdited()
    {
        var command = BuildCommand(authorizedUserStatus: UserStatus.Rejected, userToEditStatus: UserStatus.Rejected);

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.AreEqual(UserStatus.Unapproved, result.Status);
    }

    [TestMethod]
    public async Task RejectedUserStatusCanBeExplicitlySet()
    {
        // When an admin explicitly sets status, it should not become unapproved
        var command = BuildCommand(authorizedUserStatus: UserStatus.Admin, userToEditStatus: UserStatus.Rejected, isEditingSelf: false, status: UserStatus.Approved);

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.AreEqual(UserStatus.Approved, result.Status);
    }

    [TestMethod]
    public async Task UserCanEditThemselves()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(isEditingSelf: true));

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task AdminCanEditOtherUsers()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(authorizedUserStatus: UserStatus.Admin, isEditingSelf: false));

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task NonAdminCannotEditOtherUsers()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(authorizedUserStatus: UserStatus.Approved, isEditingSelf: false));

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task OnlyAdminsCanChangeStatus()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(authorizedUserStatus: UserStatus.Approved, status: UserStatus.Inactive));

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task AdminsCanChangeStatus()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(authorizedUserStatus: UserStatus.Admin, status: UserStatus.Inactive));

        Assert.IsTrue(access);
    }
}

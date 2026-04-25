using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Queries;

[TestClass]
public class GetFileForDownloadQueryTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<ApplicationSettings> testSettingsRepository;
    private readonly GetFileForDownloadQueryHandler underTest;

    public GetFileForDownloadQueryTests()
    {
        testUserRepository = new();
        testSettingsRepository = new();
        underTest = new(testUserRepository, testSettingsRepository);
    }

    private GetFileForDownloadQuery BuildQuery(UserStatus userStatus = UserStatus.Approved, string rootPath = "/downloads/public", string relativePath = "file.txt")
    {
        var user = RegisteredUser.Default with
        {
            Status = userStatus
        };
        testUserRepository.AddEntryForTest(user);

        var settings = ApplicationSettings.Default with
        {
            DownloadPaths =
            [
                new DirectoryConfiguration("/downloads/public", "Public"),
                new DirectoryConfiguration("/downloads/private", "Private")
            ]
        };
        testSettingsRepository.AddEntryForTest(settings);

        var rootDirectory = new DirectoryConfiguration(rootPath, "Test");
        return new(user.Key, rootDirectory, relativePath);
    }

    // Note: Testing actual file download would require integration tests with a real file system.
    // These tests focus on access control validation and security checks.

    [TestMethod]
    public async Task ApprovedUsersHaveAccessToAllowedDirectory()
    {
        var query = BuildQuery(UserStatus.Approved, "/downloads/public", "document.pdf");

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task AdminsHaveAccessToAllowedDirectory()
    {
        var query = BuildQuery(UserStatus.Admin, "/downloads/private", "secret.txt");

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task UnapprovedUsersDoNotHaveAccess()
    {
        var query = BuildQuery(UserStatus.Unapproved, "/downloads/public", "file.txt");

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task RejectedUsersDoNotHaveAccess()
    {
        var query = BuildQuery(UserStatus.Rejected, "/downloads/public", "file.txt");

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task AccessDeniedForUnauthorizedDirectory()
    {
        var query = BuildQuery(UserStatus.Approved, "/unauthorized/path", "file.txt");

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task PathTraversalAttackIsBlocked()
    {
        // Attempt to access a file outside the allowed directory using path traversal
        var query = BuildQuery(UserStatus.Approved, "/downloads/public", "../../../etc/passwd");

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }
}

using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class EditApplicationSettingsCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<ApplicationSettings> testAppSettingsRepository;

    private readonly EditApplicationSettingsCommandHandler underTest;

    public EditApplicationSettingsCommandTests()
    {
        testUserRepository = new();
        testAppSettingsRepository = new();

        underTest = new(testUserRepository, testAppSettingsRepository);
    }

    private EditApplicationSettingsCommand BuildCommand(UserStatus userStatus = UserStatus.Admin)
    {
        var user = RegisteredUser.Default with
        {
            Status = userStatus
        };

        testUserRepository.AddEntryForTest(user);

        var downloadPaths = new[]
        {
            new DirectoryConfiguration("/downloads/path1", "Downloads 1"),
            new DirectoryConfiguration("/downloads/path2", "Downloads 2")
        };

        var uploadPaths = new[]
        {
            new DirectoryConfiguration("/uploads/path1", "Uploads 1")
        };

        return new(downloadPaths, uploadPaths, user.Key);
    }

    [TestMethod]
    public async Task ApplicationSettingsAreUpdated()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testAppSettingsRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task DownloadPathsAreSet()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var result = testAppSettingsRepository.SavedEntries.First();

        Assert.AreEqual(2, result.DownloadPaths.Count());
        Assert.IsTrue(result.DownloadPaths.Any(p => p.Path == "/downloads/path1"));
        Assert.IsTrue(result.DownloadPaths.Any(p => p.Path == "/downloads/path2"));
    }

    [TestMethod]
    public async Task UploadPathsAreSet()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var result = testAppSettingsRepository.SavedEntries.First();

        Assert.AreEqual(1, result.UploadPaths.Count());
        Assert.IsTrue(result.UploadPaths.Any(p => p.Path == "/uploads/path1"));
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

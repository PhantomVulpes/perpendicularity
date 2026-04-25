using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Queries;

[TestClass]
public class GetApplicationSettingsQueryTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<ApplicationSettings> testSettingsRepository;
    private readonly GetApplicationSettingsQueryHandler underTest;

    public GetApplicationSettingsQueryTests()
    {
        testUserRepository = new();
        testSettingsRepository = new();
        underTest = new(testUserRepository, testSettingsRepository);
    }

    private GetApplicationSettingsQuery BuildQuery(UserStatus authenticatedUserStatus = UserStatus.Admin)
    {
        var authenticatedUser = RegisteredUser.Default with
        {
            Status = authenticatedUserStatus
        };
        testUserRepository.AddEntryForTest(authenticatedUser);

        return new(authenticatedUser.Key);
    }

    [TestMethod]
    public async Task QueryReturnsApplicationSettings()
    {
        var settings = ApplicationSettings.Default with
        {
            DownloadPaths = [new DirectoryConfiguration("/downloads", "Downloads")],
            UploadPaths = [new DirectoryConfiguration("/uploads", "Uploads")]
        };
        testSettingsRepository.AddEntryForTest(settings);

        var query = BuildQuery();
        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(ApplicationSettings.GlobalApplicationSettingsKey, result.Key);
        Assert.AreEqual(1, result.DownloadPaths.Count());
        Assert.AreEqual(1, result.UploadPaths.Count());
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

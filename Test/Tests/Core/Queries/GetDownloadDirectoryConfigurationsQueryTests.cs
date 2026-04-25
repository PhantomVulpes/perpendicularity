using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Queries;

[TestClass]
public class GetDownloadDirectoryConfigurationsQueryTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<ApplicationSettings> testAppSettingsRepository;
    private readonly GetDownloadDirectoryConfigurationsQueryHandler underTest;

    public GetDownloadDirectoryConfigurationsQueryTests()
    {
        testUserRepository = new();
        testAppSettingsRepository = new();
        underTest = new(testUserRepository, testAppSettingsRepository);
    }

    private GetDownloadDirectoryConfigurationsQuery BuildQuery(UserStatus userStatus = UserStatus.Approved)
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
        testAppSettingsRepository.AddEntryForTest(settings);

        return new(user.Key);
    }

    [TestMethod]
    public async Task QueryReturnsDownloadDirectories()
    {
        var query = BuildQuery();

        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(d => d.Alias == "Public"));
        Assert.IsTrue(result.Any(d => d.Alias == "Private"));
    }

    [TestMethod]
    public async Task ApprovedUsersHaveAccess()
    {
        var query = BuildQuery(UserStatus.Approved);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task AdminsHaveAccess()
    {
        var query = BuildQuery(UserStatus.Admin);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task UnapprovedUsersDoNotHaveAccess()
    {
        var query = BuildQuery(UserStatus.Unapproved);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task RejectedUsersDoNotHaveAccess()
    {
        var query = BuildQuery(UserStatus.Rejected);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }
}

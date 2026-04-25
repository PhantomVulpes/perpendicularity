using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class UploadFileCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<ApplicationSettings> testSettingsRepository;

    private readonly UploadFileCommandHandler underTest;

    private string testUploadPath = string.Empty;

    public UploadFileCommandTests()
    {
        testUserRepository = new();
        testSettingsRepository = new();

        underTest = new(testUserRepository, testSettingsRepository);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        // Create a temporary directory for testing uploads
        testUploadPath = Path.Combine(Path.GetTempPath(), $"perpendicularity-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(testUploadPath);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Clean up the test directory
        if (Directory.Exists(testUploadPath))
        {
            Directory.Delete(testUploadPath, true);
        }
    }

    private UploadFileCommand BuildCommand(UserStatus userStatus = UserStatus.Approved, string relativeFilePath = "testfile.txt", bool configureAllowedPath = true)
    {
        var user = RegisteredUser.Default with
        {
            Status = userStatus
        };

        testUserRepository.AddEntryForTest(user);

        var destinationDirectory = new DirectoryConfiguration(testUploadPath, "Test Uploads");

        if (configureAllowedPath)
        {
            var settings = ApplicationSettings.Default with
            {
                UploadPaths = [destinationDirectory]
            };
            testSettingsRepository.AddEntryForTest(settings);
        }
        else
        {
            var settings = ApplicationSettings.Default with
            {
                UploadPaths = []
            };
            testSettingsRepository.AddEntryForTest(settings);
        }

        var fileContent = "test file content";
        var fileStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

        return new(user.Key, destinationDirectory, relativeFilePath, fileStream, "testfile.txt", fileContent.Length);
    }

    [TestMethod]
    public async Task FileIsCreated()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var expectedFilePath = Path.Combine(testUploadPath, "testfile.txt");
        Assert.IsTrue(File.Exists(expectedFilePath));
    }

    [TestMethod]
    public async Task FileContentIsCorrect()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var expectedFilePath = Path.Combine(testUploadPath, "testfile.txt");
        var content = await File.ReadAllTextAsync(expectedFilePath);
        Assert.AreEqual("test file content", content);
    }

    [TestMethod]
    public async Task UploadMetricIsAddedToUser()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.IsTrue(result.UploadMetrics.Any());
        Assert.AreEqual(1, result.UploadMetrics.Count());
    }

    [TestMethod]
    public async Task ApprovedUsersHaveAccess()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(UserStatus.Approved));

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task AdminUsersHaveAccess()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(UserStatus.Admin));

        Assert.IsTrue(access);
    }

    [TestMethod]
    public async Task UnapprovedUsersDoNotHaveAccess()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(UserStatus.Unapproved));

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task DisallowedPathsAreRejected()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(configureAllowedPath: false));

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task PathTraversalIsBlocked()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(relativeFilePath: "../../../etc/passwd"));

        Assert.IsFalse(access);
    }
}

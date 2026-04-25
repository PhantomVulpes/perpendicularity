using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class InitializeApplicationSettingsCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<ApplicationSettings> testSettingsRepository;

    private readonly InitializeApplicationSettingsCommandHandler underTest;

    public InitializeApplicationSettingsCommandTests()
    {
        testUserRepository = new();
        testSettingsRepository = new();

        underTest = new(testUserRepository, testSettingsRepository);
    }

    private InitializeApplicationSettingsCommand BuildCommand(UserStatus userStatus = UserStatus.Admin, bool settingsAlreadyExist = false)
    {
        var user = RegisteredUser.Default with
        {
            Status = userStatus
        };

        testUserRepository.AddEntryForTest(user);

        if (settingsAlreadyExist)
        {
            var settings = ApplicationSettings.Default;
            testSettingsRepository.AddEntryForTest(settings);
        }

        return new(user.Key);
    }

    [TestMethod]
    public async Task ApplicationSettingsAreInserted()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testSettingsRepository.InsertedEntries.Count);
    }

    [TestMethod]
    public async Task SettingsHaveCorrectGlobalKey()
    {
        var command = BuildCommand();

        await underTest.ExecuteAsync(command);

        var result = testSettingsRepository.InsertedEntries.First();

        Assert.AreEqual(ApplicationSettings.GlobalApplicationSettingsKey, result.Key);
    }

    [TestMethod]
    public async Task NonAdminsDoNotHaveAccess()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(UserStatus.Approved));

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task CannotInitializeIfAlreadyExists()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(settingsAlreadyExist: true));

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task AdminsCanInitializeWhenNotExists()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand(UserStatus.Admin, settingsAlreadyExist: false));

        Assert.IsTrue(access);
    }
}

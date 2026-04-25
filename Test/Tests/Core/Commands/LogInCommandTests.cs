using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class LogInCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    private readonly LogInCommandHandler underTest;

    public LogInCommandTests()
    {
        testUserRepository = new();

        underTest = new(testUserRepository);
    }

    private LogInCommand BuildCommand()
    {
        var user = RegisteredUser.Default with
        {
            LastLoginDate = DateTime.MinValue
        };

        testUserRepository.AddEntryForTest(user);

        return new(user);
    }

    [TestMethod]
    public async Task LastLoginDateGetsUpdated()
    {
        var command = BuildCommand();

        var beforeLoginDate = DateTime.UtcNow;

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.IsTrue(result.LastLoginDate >= beforeLoginDate);
        Assert.IsTrue(result.LastLoginDate <= DateTime.UtcNow);
    }

    [TestMethod]
    public async Task AnyoneCanLogIn()
    {
        var access = await underTest.ValidateAccessAsync(BuildCommand());

        Assert.IsTrue(access);
    }
}

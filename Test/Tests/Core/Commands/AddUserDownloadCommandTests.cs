using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class AddUserDownloadCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    private readonly AddUserDownloadCommandHandler underTest;

    public AddUserDownloadCommandTests()
    {
        testUserRepository = new();

        underTest = new(testUserRepository);
    }

    private AddUserDownloadCommand BuildCommand()
    {
        var user = RegisteredUser.Default;
        testUserRepository.AddEntryForTest(user);

        return new(user.Key, [DownloadMetric.Default with { Path = "the road less traveled", SizeBytes = 69420 }]);
    }

    [TestMethod]
    public async Task DownloadMetricGetsAddedToUser()
    {
        var command = BuildCommand();

        // First we ensure there are no download metrics.
        Assert.IsFalse(testUserRepository.Entries.First().Value.DownloadMetrics.Any());

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.IsTrue(result.DownloadMetrics.Any());
    }
}

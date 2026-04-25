using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Perpendicularity.Test.Tests.Core.Commands;

[TestClass]
public class AddUserUploadCommandTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    private readonly AddUserUploadCommandHandler underTest;

    public AddUserUploadCommandTests()
    {
        testUserRepository = new();

        underTest = new(testUserRepository);
    }

    private AddUserUploadCommand BuildCommand()
    {
        var user = RegisteredUser.Default;
        testUserRepository.AddEntryForTest(user);

        return new(user.Key, [UploadMetric.Default with { Path = "the road less traveled", SizeBytes = 69420 }]);
    }

    [TestMethod]
    public async Task UploadMetricGetsAddedToUser()
    {
        var command = BuildCommand();

        // First we ensure there are no upload metrics.
        Assert.IsFalse(testUserRepository.Entries.First().Value.UploadMetrics.Any());

        await underTest.ExecuteAsync(command);

        var result = testUserRepository.SavedEntries.First();

        Assert.IsTrue(result.UploadMetrics.Any());
    }
}

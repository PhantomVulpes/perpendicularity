using Vulpes.Perpendicularity.Core.DomainExtensions;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Test.Tests.Core.DomainExtensions;

[TestClass]
public class RegisteredUserExtensionsTests
{
    [TestMethod]
    public void WithDownloadMetricAddsRange()
    {
        var user = RegisteredUser.Default;

        // First there should be none.
        Assert.IsFalse(user.DownloadMetrics.Any());

        var toBeAdded = new List<DownloadMetric>() { DownloadMetric.Default, DownloadMetric.Default };
        var result = user.WithDownloadMetric(toBeAdded);

        // Now we have multiple.
        Assert.AreEqual(toBeAdded.Count, result.DownloadMetrics.Count());
    }

    [TestMethod]
    public void WithDownloadMetricMaintainsExistingMetrics()
    {
        var user = RegisteredUser.Default with { DownloadMetrics = [DownloadMetric.Default] };

        var toBeAdded = new List<DownloadMetric>() { DownloadMetric.Default, DownloadMetric.Default };

        var result = user.WithDownloadMetric(toBeAdded);

        // We should have the original plus the new ones.
        Assert.AreEqual(result.DownloadMetrics.Count(), toBeAdded.Count + user.DownloadMetrics.Count());
    }

    [TestMethod]
    public void WithUploadMetricAddsRange()
    {
        var user = RegisteredUser.Default;

        // First there should be none.
        Assert.IsFalse(user.UploadMetrics.Any());

        var toBeAdded = new List<UploadMetric>() { UploadMetric.Default, UploadMetric.Default };
        var result = user.WithUploadMetric(toBeAdded);

        // Now we have multiple.
        Assert.AreEqual(toBeAdded.Count, result.UploadMetrics.Count());
    }

    [TestMethod]
    public void WithUploadMetricMaintainsExistingMetrics()
    {
        var user = RegisteredUser.Default with { UploadMetrics = [UploadMetric.Default] };

        var toBeAdded = new List<UploadMetric>() { UploadMetric.Default, UploadMetric.Default };

        var result = user.WithUploadMetric(toBeAdded);

        // We should have the original plus the new ones.
        Assert.AreEqual(result.UploadMetrics.Count(), toBeAdded.Count + user.UploadMetrics.Count());
    }
}
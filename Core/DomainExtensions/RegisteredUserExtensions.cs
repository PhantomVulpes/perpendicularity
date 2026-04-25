using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.DomainExtensions;

public static class RegisteredUserExtensions
{
    public static RegisteredUser WithDownloadMetric(this RegisteredUser registeredUser, IEnumerable<DownloadMetric> downloadMetrics)
    {
        var downloadMetricsList = registeredUser.DownloadMetrics.ToList();
        downloadMetricsList.AddRange(downloadMetrics);

        return registeredUser with
        {
            DownloadMetrics = downloadMetricsList
        };
    }

    public static RegisteredUser WithDownloadMetric(this RegisteredUser registeredUser, DownloadMetric downloadMetric) => registeredUser.WithDownloadMetric([downloadMetric]);
}

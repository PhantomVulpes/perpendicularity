namespace Vulpes.Perpendicularity.Core.Models;

public record ApplicationSettings : AggregateRoot
{
    public static Guid GlobalApplicationSettingsKey { get; } = Guid.Parse("003e7a5d-cd4a-4ff5-9666-4768bf828fc6");

    public static ApplicationSettings Empty { get; } = new();
    public static ApplicationSettings Default { get; } = Empty with
    {
        Key = GlobalApplicationSettingsKey
    };

    /// <summary>
    /// The paths that approved users are able to access for downloads.
    /// </summary>
    public IEnumerable<DirectoryConfiguration> DownloadPaths { get; init; } = [];
}

public record DirectoryConfiguration(string Path, string Alias);
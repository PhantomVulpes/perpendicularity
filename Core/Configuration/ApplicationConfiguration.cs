namespace Vulpes.Perpendicularity.Core.Configuration;

public static class ApplicationConfiguration
{
    private static readonly string debugEnvironmentName = "Debug";
    private static readonly string releaseEnvironmentName = "Release";

    public static string ApplicationName => "Perpendicularity";
    public static string Environment
    {
        get
        {
            // Determine the environment based on the build configuration.
#if DEBUG
            return debugEnvironmentName;
#elif RELEASE
            return releaseEnvironmentName;
#endif
        }
    }

    public static bool IsRelease => Environment == releaseEnvironmentName;

    public static string DatabaseName
    {
        get
        {
            var defaultName = ApplicationName;
#if DEBUG
            return $"{defaultName}-{Environment}";
#elif RELEASE
            return defaultName;
#endif
        }
    }

    public static string DatabaseConnectionString => "mongodb://localhost:63002/";

    public static string Version => "Alpha 0.1";
}
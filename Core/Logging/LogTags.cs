namespace Vulpes.Perpendicularity.Core.Logging;

public static class LogTags
{
    // General tags
    public static string Success => $"[{nameof(Success)}]";
    public static string Warning => $"[{nameof(Warning)}]";
    public static string Failure => $"[{nameof(Failure)}]";
    public static string UnimplementedMethod => $"[{nameof(UnimplementedMethod)}]";

    // Database Tags
    public static string DatabaseOpened => $"[{nameof(DatabaseOpened)}]";
    public static string QueryReport => $"[{nameof(QueryReport)}]";
}
namespace Vulpes.Perpendicularity.Core.QueriedModels;

public record DirectoryContents
{
    public static DirectoryContents Empty { get; } = new();
    public static DirectoryContents Default(string currentDirectory) => Empty with
    {
        CurrentDirectory = currentDirectory,
    };

    public string CurrentDirectory { get; init; } = string.Empty;
    public IEnumerable<string> Directories { get; init; } = [];
    public IEnumerable<string> Files { get; init; } = [];
}
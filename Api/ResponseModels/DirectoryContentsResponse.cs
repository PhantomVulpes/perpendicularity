using Vulpes.Perpendicularity.Core.QueriedModels;

namespace Vulpes.Perpendicularity.Api.ResponseModels;

public record DirectoryContentsResponse(IEnumerable<string> Directories, IEnumerable<string> Files)
{
    public static DirectoryContentsResponse FromDirectoryContents(DirectoryContents directoryContents)
    {
        var sortedDirectories = directoryContents.Directories.OrderBy(d => d).ToList();
        var sortedFiles = directoryContents.Files.OrderBy(f => f).ToList();

        return new(sortedDirectories, sortedFiles);
    }
}
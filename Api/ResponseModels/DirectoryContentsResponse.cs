using Vulpes.Perpendicularity.Core.QueriedModels;

namespace Vulpes.Perpendicularity.Api.ResponseModels;

public record DirectoryContentsResponse(IEnumerable<string> Directories, IEnumerable<string> Files)
{
    public static DirectoryContentsResponse FromDirectoryContents(DirectoryContents directoryContents) => new(directoryContents.Directories, directoryContents.Files);
}
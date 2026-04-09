namespace Vulpes.Perpendicularity.Api.RequestModels;

public record DownloadFilesAsZipRequest(string RootDirectory, IEnumerable<string> FilePaths);

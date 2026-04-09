using System.IO.Compression;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.QueriedModels;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetFilesAsZipQuery(Guid AuthenticatedUserKey, DirectoryConfiguration RootDirectory, IEnumerable<string> RelativeFilePaths) : Query;

public class GetFilesAsZipQueryHandler : QueryHandler<GetFilesAsZipQuery, ZipFileForDownload>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public GetFilesAsZipQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }

    protected override async Task<ZipFileForDownload> InternalRequestAsync(GetFilesAsZipQuery query)
    {
        var user = await userRepository.GetAsync(query.AuthenticatedUserKey);
        if (user.Status != UserStatus.Admin && user.Status != UserStatus.Approved)
        {
            AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} does not have access to download files.").ThrowIfAccessDenied();
        }

        // Check the root directory is actually allowed
        var settings = await settingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);
        if (!settings.DownloadPaths.Select(value => value.Path).Contains(query.RootDirectory.Path))
        {
            AccessResult.Fail($"Root directory {query.RootDirectory.Path} is not an allowed download path.").ThrowIfAccessDenied();
        }

        var normalizedRootPath = Path.GetFullPath(query.RootDirectory.Path);
        var validatedFiles = new List<(string FullPath, string RelativePath)>();

        // Validate all files and collect their paths
        foreach (var relativeFilePath in query.RelativeFilePaths)
        {
            var fullFilePath = Path.Combine(query.RootDirectory.Path, relativeFilePath);
            var normalizedFullPath = Path.GetFullPath(fullFilePath);

            // Security check: Ensure the resolved path is still within the allowed root directory
            if (!normalizedFullPath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
            {
                AccessResult.Fail($"Attempted path traversal attack detected for file: {relativeFilePath}").ThrowIfAccessDenied();
            }

            // Validate the file exists
            if (!File.Exists(normalizedFullPath))
            {
                throw new FileNotFoundException($"File not found: {relativeFilePath}");
            }

            validatedFiles.Add((normalizedFullPath, relativeFilePath));
        }

        // Create a temporary ZIP file
        var tempZipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");

        try
        {
            using (var zipArchive = ZipFile.Open(tempZipPath, ZipArchiveMode.Create))
            {
                foreach (var (fullPath, relativePath) in validatedFiles)
                {
                    // Normalize the entry name (replace backslashes with forward slashes for cross-platform compatibility)
                    var entryName = relativePath.Replace('\\', '/');
                    zipArchive.CreateEntryFromFile(fullPath, entryName, CompressionLevel.Optimal);
                }
            }

            // Generate a meaningful filename
            var zipFileName = query.RelativeFilePaths.Count() == 1
                ? $"{Path.GetFileNameWithoutExtension(query.RelativeFilePaths.First())}.zip"
                : $"download_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";

            return new ZipFileForDownload(tempZipPath, zipFileName);
        }
        catch
        {
            // Clean up the temp file if something went wrong
            if (File.Exists(tempZipPath))
            {
                File.Delete(tempZipPath);
            }
            throw;
        }
    }
}

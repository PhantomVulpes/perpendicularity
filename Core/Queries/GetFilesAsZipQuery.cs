using System.IO.Compression;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.QueriedModels;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetFilesAsZipQuery(Guid AuthenticatedUserKey, DirectoryConfiguration RootDirectory, IEnumerable<string> RelativeFilePaths) : Query<ZipFileForDownload>;

public class GetFilesAsZipQueryHandler : QueryHandler<GetFilesAsZipQuery, ZipFileForDownload>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public GetFilesAsZipQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }

    protected override Task<ZipFileForDownload> InternalRequestAsync(GetFilesAsZipQuery query)
    {
        var validatedFiles = GetValidatedFiles(query.RootDirectory.Path, query.RelativeFilePaths, validateAccess: false);

        // Create a temporary ZIP file
        var tempZipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");

        try
        {
            using (var zipArchive = ZipFile.Open(tempZipPath, ZipArchiveMode.Create))
            {
                foreach (var (_, fullPath, relativePath) in validatedFiles)
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

            return new ZipFileForDownload(tempZipPath, zipFileName).FromResult();
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

    protected async override Task<AccessResult> InternalValidateAccessAsync(GetFilesAsZipQuery query)
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

        // Validate the files and ensure they are within the allowed root directory.
        var access = GetValidatedFiles(query.RootDirectory.Path, query.RelativeFilePaths, validateAccess: true).Select(tuple => tuple.AccessResult).Where(result => !result.AccessGranted);
        if (access.Any())
        {
            return access.First();
        }

        return AccessResult.Success();
    }

    private static List<(AccessResult AccessResult, string FullPath, string RelativePath)> GetValidatedFiles(string rootDirectoryPath, IEnumerable<string> relativeFilePaths, bool validateAccess)
    {
        var normalizedRootPath = Path.GetFullPath(rootDirectoryPath);
        var validatedFiles = new List<(AccessResult AccessResult, string FullPath, string RelativePath)>();

        // Validate all files and collect their paths
        foreach (var relativeFilePath in relativeFilePaths)
        {
            var fullFilePath = Path.Combine(rootDirectoryPath, relativeFilePath);
            var normalizedFullPath = Path.GetFullPath(fullFilePath);

            // Security check: Ensure the resolved path is still within the allowed root directory
            if (validateAccess && !normalizedFullPath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
            {
                return [(AccessResult.Fail($"Attempted path traversal attack detected for file: {relativeFilePath}"), string.Empty, string.Empty)];
            }

            // Validate the file exists
            if (!File.Exists(normalizedFullPath))
            {
                throw new FileNotFoundException($"File not found: {relativeFilePath}");
            }

            validatedFiles.Add((AccessResult.Success(), normalizedFullPath, relativeFilePath));
        }

        return validatedFiles;
    }
}
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.QueriedModels;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetFileForDownloadQuery(Guid AuthenticatedUserKey, DirectoryConfiguration RootDirectory, string RelativeFilePath) : Query<FileForDownload>;

public class GetFileForDownloadQueryHandler : QueryHandler<GetFileForDownloadQuery, FileForDownload>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public GetFileForDownloadQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }

    protected override Task<FileForDownload> InternalRequestAsync(GetFileForDownloadQuery query)
    {
        var (normalizedFullPath, _) = GetPaths(query.RelativeFilePath, query.RootDirectory.Path);

        // Validate the file exists
        if (!File.Exists(normalizedFullPath))
        {
            throw new FileNotFoundException($"File not found: {query.RelativeFilePath}");
        }

        return new FileForDownload(normalizedFullPath, Path.GetFileName(normalizedFullPath)).FromResult();
    }

    protected async override Task<AccessResult> InternalValidateAccessAsync(GetFileForDownloadQuery query)
    {
        var user = await userRepository.GetAsync(query.AuthenticatedUserKey);
        if (user.Status != UserStatus.Admin && user.Status != UserStatus.Approved)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} does not have access to download files.");
        }

        // Check the root directory is actually allowed
        var settings = await settingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);
        if (!settings.DownloadPaths.Select(value => value.Path).Contains(query.RootDirectory.Path))
        {
            return AccessResult.Fail($"Root directory {query.RootDirectory.Path} is not an allowed download path.");
        }

        // Security check: Ensure the resolved path is still within the allowed root directory
        var (normalizedFullPath, normalizedRootPath) = GetPaths(query.RelativeFilePath, query.RootDirectory.Path);

        if (!normalizedFullPath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
        {
            return AccessResult.Fail("Attempted path traversal attack detected.");
        }

        return AccessResult.Success();
    }

    private static (string normalizedFullPath, string normalizedRootPath) GetPaths(string relativeFilePath, string rootDirectoryPath)
    {
        var fullFilePath = Path.Combine(rootDirectoryPath, relativeFilePath);

        var normalizedFullPath = Path.GetFullPath(fullFilePath);
        var normalizedRootPath = Path.GetFullPath(rootDirectoryPath);
        return (normalizedFullPath, normalizedRootPath);
    }

}

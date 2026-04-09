using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.QueriedModels;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetFileForDownloadQuery(Guid AuthenticatedUserKey, DirectoryConfiguration RootDirectory, string RelativeFilePath) : Query;

public class GetFileForDownloadQueryHandler : QueryHandler<GetFileForDownloadQuery, FileForDownload>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public GetFileForDownloadQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }

    protected override async Task<FileForDownload> InternalRequestAsync(GetFileForDownloadQuery query)
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

        // Construct the full file path
        var fullFilePath = Path.Combine(query.RootDirectory.Path, query.RelativeFilePath);

        // Security check: Ensure the resolved path is still within the allowed root directory
        var normalizedFullPath = Path.GetFullPath(fullFilePath);
        var normalizedRootPath = Path.GetFullPath(query.RootDirectory.Path);

        if (!normalizedFullPath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
        {
            AccessResult.Fail("Attempted path traversal attack detected.").ThrowIfAccessDenied();
        }

        // Validate the file exists
        if (!File.Exists(normalizedFullPath))
        {
            throw new FileNotFoundException($"File not found: {query.RelativeFilePath}");
        }

        return new FileForDownload(normalizedFullPath, Path.GetFileName(normalizedFullPath));
    }
}

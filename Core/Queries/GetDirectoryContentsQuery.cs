using System;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetDirectoryContentsQuery(Guid AuthenticatedUserKey, DirectoryConfiguration RootDirectory, string RemainingPath) : Query;
public class GetDirectoryContentsQueryHandler : QueryHandler<GetDirectoryContentsQuery, IEnumerable<string>>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public GetDirectoryContentsQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }


    protected override async Task<IEnumerable<string>> InternalRequestAsync(GetDirectoryContentsQuery query)
    {
        var user = await userRepository.GetAsync(query.AuthenticatedUserKey);
        if (user.Status != UserStatus.Admin && user.Status != UserStatus.Approved)
        {
            AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} does not have access to view directory contents.").ThrowIfAccessDenied();
        }

        // Check the root directory is actually allowed.
        var settings = await settingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);
        if (!settings.DownloadPaths.Select(value => value.Path).Contains(query.RootDirectory.Path))
        {
            AccessResult.Fail($"Root directory {query.RootDirectory.Path} is not an allowed download path.").ThrowIfAccessDenied();
        }

        var currentDirectory = Path.Combine(query.RootDirectory.Path, query.RemainingPath);

        // Validate the directory exists
        if (!Directory.Exists(currentDirectory))
        {
            throw new DirectoryNotFoundException($"Directory not found: {currentDirectory}");
        }

        // Get all files and directories
        var entries = Directory.GetFileSystemEntries(currentDirectory);

        // Return just the names (not full paths)
        return entries.Select(Path.GetFileName).Where(name => !string.IsNullOrEmpty(name))!;
    }
}
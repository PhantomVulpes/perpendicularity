using System;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.QueriedModels;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetDirectoryContentsQuery(Guid AuthenticatedUserKey, DirectoryConfiguration RootDirectory, string RemainingPath) : Query<DirectoryContents>;
public class GetDirectoryContentsQueryHandler : QueryHandler<GetDirectoryContentsQuery, DirectoryContents>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public GetDirectoryContentsQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }


    protected override async Task<DirectoryContents> InternalRequestAsync(GetDirectoryContentsQuery query)
    {
        var currentDirectory = Path.Combine(query.RootDirectory.Path, query.RemainingPath);

        // Validate the directory exists
        if (!Directory.Exists(currentDirectory))
        {
            throw new DirectoryNotFoundException($"Directory not found: {currentDirectory}");
        }

        // Get all files and directories
        var files = Directory.GetFiles(currentDirectory);
        var directories = Directory.GetDirectories(currentDirectory);

        var result = DirectoryContents.Default(currentDirectory) with
        {
            Files = files.Select(Path.GetFileName).Where(name => !string.IsNullOrEmpty(name))!,
            Directories = directories.Select(Path.GetFileName).Where(name => !string.IsNullOrEmpty(name))!
        };

        return result;
    }

    protected async override Task<AccessResult> InternalValidateAccessAsync(GetDirectoryContentsQuery query)
    {
        var user = await userRepository.GetAsync(query.AuthenticatedUserKey);
        if (user.Status == UserStatus.Admin || user.Status == UserStatus.Approved)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} does not have access to view directory contents.");
        }

        // Check the root directory is actually allowed.
        var settings = await settingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);
        if (!settings.DownloadPaths.Select(value => value.Path).Contains(query.RootDirectory.Path))
        {
            return AccessResult.Fail($"Root directory {query.RootDirectory.Path} is not an allowed download path.");
        }

        return AccessResult.Success();
    }

}
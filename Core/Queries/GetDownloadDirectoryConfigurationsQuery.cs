using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetDownloadDirectoryConfigurationsQuery(Guid UserKey) : Query<IEnumerable<DirectoryConfiguration>>;
public class GetDownloadDirectoryConfigurationsQueryHandler : QueryHandler<GetDownloadDirectoryConfigurationsQuery, IEnumerable<DirectoryConfiguration>>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> appSettingsRepository;

    public GetDownloadDirectoryConfigurationsQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> appSettingsRepository)
    {
        this.userRepository = userRepository;
        this.appSettingsRepository = appSettingsRepository;
    }

    protected override async Task<IEnumerable<DirectoryConfiguration>> InternalRequestAsync(GetDownloadDirectoryConfigurationsQuery query)
    {
        // Get the application settings and return the directories.
        var settings = await appSettingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);

        return settings.DownloadPaths;
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(GetDownloadDirectoryConfigurationsQuery query)
    {
        var user = await userRepository.GetAsync(query.UserKey);
        if (user.Status == UserStatus.Admin || user.Status == UserStatus.Approved) { return AccessResult.Success(); }

        return AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} is not ${nameof(UserStatus.Approved)}.");
    }

}
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetDirectoryConfigurationsQuery(Guid UserKey) : Query<IEnumerable<DirectoryConfiguration>>;
public class GetDirectoryConfigurationsQueryHandler : QueryHandler<GetDirectoryConfigurationsQuery, IEnumerable<DirectoryConfiguration>>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> appSettingsRepository;

    public GetDirectoryConfigurationsQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> appSettingsRepository)
    {
        this.userRepository = userRepository;
        this.appSettingsRepository = appSettingsRepository;
    }

    protected override async Task<IEnumerable<DirectoryConfiguration>> InternalRequestAsync(GetDirectoryConfigurationsQuery query)
    {
        // Get the application settings and return the directories.
        var settings = await appSettingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);

        return settings.DownloadPaths;
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(GetDirectoryConfigurationsQuery query)
    {
        var user = await userRepository.GetAsync(query.UserKey);
        if (user.Status == UserStatus.Admin || user.Status == UserStatus.Approved) { return AccessResult.Success(); }

        return AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} is not ${nameof(UserStatus.Approved)}.");
    }

}
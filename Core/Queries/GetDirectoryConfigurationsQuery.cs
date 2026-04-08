using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetDirectoryConfigurationsQuery(Guid UserKey) : Query;
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
        // TODO: Some kind of throw if not accessible extension on the user? I've got a few things I need to update with Electrum too, maybe that's a waste but fuck man I need queries to be better and I need them to be securable by default.
        var user = await userRepository.GetAsync(query.UserKey);
        if (user.Status != UserStatus.Admin && user.Status != UserStatus.Approved)
        {
            AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} is not ${nameof(UserStatus.Approved)}.").ThrowIfAccessDenied();
        }

        // Get the application settings and return the directories.
        var settings = await appSettingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);

        return settings.DownloadPaths;
    }
}
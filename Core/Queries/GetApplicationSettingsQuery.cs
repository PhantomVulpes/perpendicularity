using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetApplicationSettingsQuery(Guid AuthenticatedUserKey) : Query;
public class GetApplicationSettingsQueryHandler : QueryHandler<GetApplicationSettingsQuery, ApplicationSettings>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public GetApplicationSettingsQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }

    protected override async Task<ApplicationSettings> InternalRequestAsync(GetApplicationSettingsQuery query)
    {
        var authenticatedUser = await userRepository.GetAsync(query.AuthenticatedUserKey);
        if (authenticatedUser.Status != UserStatus.Admin)
        {
            // There's no access check on queries (should be added to Electrum), so we gotta do this.
            AccessResult.Fail($"{nameof(RegisteredUser)} {authenticatedUser.ToLogName()} does not have ${nameof(UserStatus.Admin)} access.").ThrowIfAccessDenied();
        }

        return await settingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);
    }
}
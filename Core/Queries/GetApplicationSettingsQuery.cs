using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetApplicationSettingsQuery(Guid AuthenticatedUserKey) : Query<ApplicationSettings>;
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
        return await settingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);
    }

    protected async override Task<AccessResult> InternalValidateAccessAsync(GetApplicationSettingsQuery query)
    {
        var authenticatedUser = await userRepository.GetAsync(query.AuthenticatedUserKey);
        if (authenticatedUser.Status == UserStatus.Admin)
        {
            return AccessResult.Success();
        }

        return AccessResult.Fail($"{nameof(RegisteredUser)} {authenticatedUser.ToLogName()} does not have ${nameof(UserStatus.Admin)} access.");
    }

}
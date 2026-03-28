using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record InitializeApplicationSettingsCommand(Guid UserKey) : Command;
public class InitializeApplicationSettingsCommandHandler : CommandHandler<InitializeApplicationSettingsCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public InitializeApplicationSettingsCommandHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }

    protected override Task InternalExecuteAsync(InitializeApplicationSettingsCommand command)
    {
        var settings = ApplicationSettings.Default with
        {
            Key = ApplicationSettings.GlobalApplicationSettingsKey
        };

        return settingsRepository.InsertAsync(settings);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(InitializeApplicationSettingsCommand command)
    {
        var user = await userRepository.GetAsync(command.UserKey);
        var result = user.Status == UserStatus.Admin ? AccessResult.Success() : AccessResult.Fail($"Only {nameof(UserStatus.Admin)} users can initialize application settings.");

        return result;
    }
}
using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record EditApplicationSettingsCommand(IEnumerable<DirectoryConfiguration> DirectoryConfigurations, Guid UserKey) : Command;
public class EditApplicationSettingsCommandHandler : CommandHandler<EditApplicationSettingsCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> appSettingsRepository;

    public EditApplicationSettingsCommandHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> appSettingsRepository)
    {
        this.userRepository = userRepository;
        this.appSettingsRepository = appSettingsRepository;
    }

    protected override async Task InternalExecuteAsync(EditApplicationSettingsCommand command)
    {
        var newApplicationSettings = ApplicationSettings.Default with
        {
            DownloadPaths = command.DirectoryConfigurations
        };

        await appSettingsRepository.SaveAsync(newApplicationSettings.EditingToken, newApplicationSettings);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(EditApplicationSettingsCommand command)
    {
        // Only admins have access.
        var user = await userRepository.GetAsync(command.UserKey);

        return user.Status == UserStatus.Admin ? AccessResult.Success() : AccessResult.Fail($"User {user.ToLogName()} does not have access to {nameof(EditApplicationSettingsCommand)}.");
    }

}
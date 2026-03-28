using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Exceptions;
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

    protected override async Task InternalExecuteAsync(InitializeApplicationSettingsCommand command)
    {
        var settings = ApplicationSettings.Default with
        {
            Key = ApplicationSettings.GlobalApplicationSettingsKey
        };

        await settingsRepository.InsertAsync(settings);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(InitializeApplicationSettingsCommand command)
    {
        var user = await userRepository.GetAsync(command.UserKey);
        var result = user.Status == UserStatus.Admin ? AccessResult.Success() : AccessResult.Fail($"Only {nameof(UserStatus.Admin)} users can initialize application settings.");

        // Check if the application settings already exist.
        var settingsExist = true;
        try
        {
            var initialApplicationSettings = await settingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);

            // It should have thrown rather than give null but just in case.
            if (initialApplicationSettings != null)
            {
                settingsExist = true;
            }
            else
            {
                settingsExist = false;
            }
        }
        catch (PerhapsNotFoundException)
        {
            settingsExist = false;
        }

        if (settingsExist)
        {
            return AccessResult.Fail($"{nameof(ApplicationSettings)} have already been initialized. This operation can only be performed once.");
        }

        return result;
    }
}
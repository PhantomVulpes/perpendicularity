using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record LogInCommand(RegisteredUser User) : Command;
public class LogInCommandHandler : CommandHandler<LogInCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;

    public LogInCommandHandler(IModelRepository<RegisteredUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(LogInCommand command)
    {
        var updatedUser = command.User with
        {
            LastLoginDate = DateTime.UtcNow
        };

        await userRepository.SaveAsync(command.User.EditingToken, updatedUser);
    }

    protected override Task<AccessResult> InternalValidateAccessAsync(LogInCommand command) => AccessResult.Success().FromResult();

}
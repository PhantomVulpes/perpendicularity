using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record RegisterNewUserCommand(Guid Key, string FirstName, string LastName, string PasswordHash) : Command;
public class RegisterNewUserCommandHandler : CommandHandler<RegisterNewUserCommand>
{
    private readonly IModelRepository<RegisteredUser> registeredUserModelRepository;

    public RegisterNewUserCommandHandler(IModelRepository<RegisteredUser> registeredUserModelRepository)
    {
        this.registeredUserModelRepository = registeredUserModelRepository;
    }

    protected override async Task InternalExecuteAsync(RegisterNewUserCommand command)
    {
        var newUser = RegisteredUser.Default with
        {
            Key = command.Key,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = command.PasswordHash,
            Status = UserStatus.Unapproved,
        };

        // TODO: Electrum should get the Save, Insert, and Validation models for this.
        await registeredUserModelRepository.InsertAsync(newUser);
    }

    // Any user can register.
    protected override Task<AccessResult> InternalValidateAccessAsync(RegisterNewUserCommand command) => AccessResult.Success().FromResult();
}
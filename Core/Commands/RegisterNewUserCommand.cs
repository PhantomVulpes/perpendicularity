using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record RegisterNewUserCommand(Guid Key, string FirstName, string LastName, string PasswordRaw) : Command;
public class RegisterNewUserCommandHandler : CommandHandler<RegisterNewUserCommand>
{
    private readonly IModelRepository<RegisteredUser> registeredUserModelRepository;
    private readonly IKnoxHasher knoxHasher;

    public RegisterNewUserCommandHandler(IModelRepository<RegisteredUser> registeredUserModelRepository, IKnoxHasher knoxHasher)
    {
        this.registeredUserModelRepository = registeredUserModelRepository;
        this.knoxHasher = knoxHasher;
    }

    protected override async Task InternalExecuteAsync(RegisterNewUserCommand command)
    {
        var hashedPassword = knoxHasher.HashPassword(command.PasswordRaw);
        var newUser = RegisteredUser.Default with
        {
            Key = command.Key,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = new(hashedPassword),
            Status = UserStatus.Unapproved,
        };

        // TODO: Electrum should get the Save, Insert, and Validation models for this.
        await registeredUserModelRepository.InsertAsync(newUser.PrepareForInsert());
    }

    // Any user can register.
    protected override Task<AccessResult> InternalValidateAccessAsync(RegisterNewUserCommand command) => AccessResult.Success().FromResult();
}
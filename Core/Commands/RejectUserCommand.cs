using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record RejectUserCommand(Guid AuthenticatedUserKey, Guid RequestedUserKey) : Command;
public class RejectUserCommandHandler : CommandHandler<RejectUserCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;

    public RejectUserCommandHandler(IModelRepository<RegisteredUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(RejectUserCommand command)
    {
        var updatedRequestedUser = (await userRepository.GetAsync(command.RequestedUserKey)) with
        {
            Status = UserStatus.Rejected,
        };

        await userRepository.SaveAsync(updatedRequestedUser.PrepareForSave());
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(RejectUserCommand command)
    {
        var authenticatedUser = await userRepository.GetAsync(command.AuthenticatedUserKey);
        if (authenticatedUser.Status != UserStatus.Admin)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} is not {UserStatus.Admin} and cannot access {nameof(RejectUserCommand)}.");
        }

        var attemptedUser = await userRepository.GetAsync(command.RequestedUserKey);
        if (attemptedUser.Status != UserStatus.Unapproved)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} is not {UserStatus.Unapproved} and cannot be rejected.");
        }

        return AccessResult.Success();
    }
}

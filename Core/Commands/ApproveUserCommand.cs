using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record ApproveUserCommand(Guid AuthenticatedUserKey, Guid RequestedUserKey) : Command;
public class ApproveUserCommandHandler : CommandHandler<ApproveUserCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;

    public ApproveUserCommandHandler(IModelRepository<RegisteredUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(ApproveUserCommand command)
    {
        var updatedRequestedUser = (await userRepository.GetAsync(command.RequestedUserKey)) with
        {
            Status = UserStatus.Approved,
        };

        await userRepository.SaveAsync(updatedRequestedUser.PrepareForSave());
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(ApproveUserCommand command)
    {
        var authenticatedUser = await userRepository.GetAsync(command.AuthenticatedUserKey);
        if (authenticatedUser.Status != UserStatus.Admin)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} is not {UserStatus.Admin} and cannot access {nameof(ApproveUserCommand)}.");
        }

        var attemptedUser = await userRepository.GetAsync(command.RequestedUserKey);
        if (attemptedUser.Status != UserStatus.Unapproved)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} is not {UserStatus.Unapproved} and cannot be approved.");
        }

        return AccessResult.Success();
    }
}
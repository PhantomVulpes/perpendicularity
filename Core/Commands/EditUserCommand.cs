using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record EditUserCommand(Guid UserToEditKey, Guid AuthorizedUserKey, string? FirstName = null, string? LastName = null, string? PasswordRaw = null, UserStatus? Status = null) : Command;

public class EditUserCommandHandler : CommandHandler<EditUserCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IKnoxHasher knoxHasher;

    public EditUserCommandHandler(IModelRepository<RegisteredUser> userRepository, IKnoxHasher knoxHasher)
    {
        this.userRepository = userRepository;
        this.knoxHasher = knoxHasher;
    }

    protected override async Task InternalExecuteAsync(EditUserCommand command)
    {
        var userToEdit = await userRepository.GetAsync(command.UserToEditKey);

        var newStatus = command.Status ?? userToEdit.Status;
        if (userToEdit.Status == UserStatus.Rejected && command.Status != null)
        {
            // If a user was rejected and edited their profile, switch back to unapproved for an admin to review again.
            newStatus = UserStatus.Unapproved;
        }

        var updatedUser = userToEdit with
        {
            FirstName = command.FirstName ?? userToEdit.FirstName,
            LastName = command.LastName ?? userToEdit.LastName,
            PasswordHash = command.PasswordRaw is not null
                ? new(knoxHasher.HashPassword(command.PasswordRaw))
                : userToEdit.PasswordHash,
            Status = newStatus
        };

        await userRepository.SaveAsync(updatedUser.PrepareForSave());
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(EditUserCommand command)
    {
        var authorizedUser = await userRepository.GetAsync(command.AuthorizedUserKey);
        var isEditingSelf = command.UserToEditKey == command.AuthorizedUserKey;
        var isAdmin = authorizedUser.Status == UserStatus.Admin;

        // User can edit themselves, or admin can edit anyone
        if (!isEditingSelf && !isAdmin)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} is not authorized to edit another user.");
        }

        // Only admins can change user status
        if (command.Status is not null && !isAdmin)
        {
            return AccessResult.Fail($"Only {UserStatus.Admin} users can change {nameof(UserStatus)}.");
        }

        return AccessResult.Success();
    }
}

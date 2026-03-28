using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record ApproveUserRequest
{
    public Guid RequestedUserKey { get; init; } = Guid.Empty;

    public ApproveUserCommand ToCommand(RegisteredUser authenticatedUser) => new(authenticatedUser.Key, RequestedUserKey);
}

using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record RejectUserRequest
{
    public Guid RequestedUserKey { get; init; } = Guid.Empty;

    public RejectUserCommand ToCommand(RegisteredUser authenticatedUser) => new(authenticatedUser.Key, RequestedUserKey);
}

using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record EditUserRequest
{
    public Guid UserToEditKey { get; init; } = Guid.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PasswordRaw { get; init; }
    public UserStatus? Status { get; init; }

    public EditUserCommand ToCommand(RegisteredUser authorizedUser) => new(
        UserToEditKey,
        authorizedUser.Key,
        FirstName,
        LastName,
        PasswordRaw,
        Status
    );
}

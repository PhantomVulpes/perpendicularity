using Vulpes.Perpendicularity.Core.Commands;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record RegisterNewUserRequest(string FirstName, string LastName, string PasswordRaw)
{
    public RegisterNewUserCommand ToCommand() => new(Guid.NewGuid(), FirstName, LastName, PasswordRaw);
}

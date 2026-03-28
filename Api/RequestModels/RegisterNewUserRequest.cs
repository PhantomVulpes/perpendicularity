using Vulpes.Perpendicularity.Core.Commands;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record RegisterNewUserRequest(string FirstName, string LastName, string PasswordHash)
{
    public RegisterNewUserCommand ToCommand() => new(Guid.NewGuid(), FirstName, LastName, PasswordHash);
}

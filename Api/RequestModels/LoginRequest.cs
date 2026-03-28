using Vulpes.Perpendicularity.Core.Queries;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record LoginRequest(string FirstName, string LastName, string PasswordRaw)
{
    public GetUserByLoginCredentialsQuery ToQuery() => new(FirstName, LastName, PasswordRaw);
}

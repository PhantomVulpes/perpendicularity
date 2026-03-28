using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record LoginResponse
{
    public static LoginResponse Empty { get; } = new();

    public string Token { get; init; } = string.Empty;
    public string UserKey { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public UserStatus Status { get; init; } = UserStatus.Unknown;

    public static LoginResponse FromRegisteredUser(RegisteredUser user, string token) => Empty with
    {
        Token = token,
        UserKey = user.Key.ToString(),
        FirstName = user.FirstName,
        LastName = user.LastName,
        DisplayName = user.DisplayName,
        Status = user.Status
    };
}
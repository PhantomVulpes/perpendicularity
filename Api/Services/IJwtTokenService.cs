using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.Services;

public interface IJwtTokenService
{
    string GenerateToken(RegisteredUser user);
}

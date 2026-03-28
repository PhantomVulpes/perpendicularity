using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vulpes.Perpendicularity.Api.Middleware;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class PerpendicularityController : ControllerBase
{
    public RegisteredUser RegisteredUser => (HttpContext.Items[UserContextMiddleware.ContextKey] as RegisteredUser)!;
}

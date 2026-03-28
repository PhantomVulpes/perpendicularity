using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Api.RequestModels;

namespace Vulpes.Perpendicularity.Api.Controllers;

public class UserController : PerpendicularityController
{
    private readonly IMediator mediator;

    public UserController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> RegisterUserAsync(RegisterNewUserRequest request)
    {
        var command = request.ToCommand();
        await mediator.ExecuteCommandAsync(command);

        return Ok(command.Key.ToString());
    }
}
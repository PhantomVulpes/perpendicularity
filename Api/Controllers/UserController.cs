using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Api.RequestModels;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;

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

    [HttpPost("login")]
    [ProducesResponseType(typeof(RegisteredUser), StatusCodes.Status200OK)]
    public async Task<ActionResult<RegisteredUser>> LoginAsync(LoginRequest request)
    {
        var query = request.ToQuery();
        var user = await mediator.RequestResponseAsync<GetUserByLoginCredentialsQuery, RegisteredUser>(query);

        return Ok(user);
    }
}
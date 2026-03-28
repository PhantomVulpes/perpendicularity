using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Api.RequestModels;
using Vulpes.Perpendicularity.Api.Services;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;

namespace Vulpes.Perpendicularity.Api.Controllers;

public class UserController : PerpendicularityController
{
    private readonly IMediator mediator;
    private readonly IJwtTokenService jwtTokenService;

    public UserController(IMediator mediator, IJwtTokenService jwtTokenService)
    {
        this.mediator = mediator;
        this.jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> RegisterUserAsync(RegisterNewUserRequest request)
    {
        var command = request.ToCommand();
        await mediator.ExecuteCommandAsync(command);

        return Ok(command.Key.ToString());
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var query = request.ToQuery();
        var user = await mediator.RequestResponseAsync<GetUserByLoginCredentialsQuery, RegisteredUser>(query);

        var token = jwtTokenService.GenerateToken(user);

        var response = LoginResponse.FromRegisteredUser(user, token);

        // TODO: Update last login time.

        return Ok(response);
    }

    [HttpGet("all")]
    [Authorize(Roles = nameof(UserStatus.Admin))]
    [ProducesResponseType(typeof(List<RegisteredUser>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RegisteredUser>>> GetAllUsersAsync()
    {
        var query = new GetAllUsersQuery(RegisteredUser.Key);
        var users = await mediator.RequestResponseAsync<GetAllUsersQuery, IQueryable<RegisteredUser>>(query);

        return Ok(users.ToList());
    }
}
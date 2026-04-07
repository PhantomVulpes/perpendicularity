using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Api.RequestModels;
using Vulpes.Perpendicularity.Core.Commands;

namespace Vulpes.Perpendicularity.Api.Controllers;

public class AdminController : PerpendicularityController
{
    private readonly IMediator mediator;

    public AdminController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("initialize-settings")]
    public async Task<ActionResult> InitializeApplicationSettingsAsync()
    {
        var command = new InitializeApplicationSettingsCommand(RegisteredUser.Key);
        await mediator.ExecuteCommandAsync(command);

        return Ok();
    }

    [HttpPost("approve-user")]
    public async Task<ActionResult> ApproveUserAsync(ApproveUserRequest request)
    {
        var command = request.ToCommand(RegisteredUser);
        await mediator.ExecuteCommandAsync(command);

        return Ok();
    }

    [HttpPost("edit-settings")]
    public async Task<ActionResult> EditApplicationSettings(EditApplicationSettingsRequest request)
    {
        var command = request.ToCommand(RegisteredUser);
        await mediator.ExecuteCommandAsync(command);

        return Ok();
    }
}

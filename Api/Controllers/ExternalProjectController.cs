using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Api.RequestModels;
using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;

namespace Vulpes.Perpendicularity.Api.Controllers;

public class ExternalProjectController : PerpendicularityController
{
    private readonly IMediator mediator;

    public ExternalProjectController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ExternalProject>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ExternalProject>>> GetAllExternalProjectsAsync()
    {
        var query = new GetAllExternalProjectsQuery();
        var projects = await mediator.RequestResponseAsync(query);

        return Ok(projects.ToList());
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserStatus.Admin))]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> AddExternalProjectAsync(AddExternalProjectRequest request)
    {
        var command = request.ToCommand(RegisteredUser);
        await mediator.ExecuteCommandAsync(command);

        return Ok(command.Key.ToString());
    }

    [HttpDelete("{projectKey}")]
    [Authorize(Roles = nameof(UserStatus.Admin))]
    public async Task<ActionResult> DeleteExternalProjectAsync(Guid projectKey)
    {
        var command = new DeleteExternalProjectCommand(projectKey, RegisteredUser.Key);
        await mediator.ExecuteCommandAsync(command);

        return Ok();
    }
}

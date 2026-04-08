using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;

namespace Vulpes.Perpendicularity.Api.Controllers;

public class FileController : PerpendicularityController
{
    private readonly IMediator mediator;

    public FileController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("directory")]
    public async Task<ActionResult<IEnumerable<string>>> GetStuff()
    {
        // TODO: GetAllowedDownloadPathsQuery instead of application settings.

        var applicationSettings = await mediator.RequestResponseAsync<GetApplicationSettingsQuery, ApplicationSettings>(new(RegisteredUser.Key));
        var result = await mediator.RequestResponseAsync<GetDirectoryContentsQuery, IEnumerable<string>>(new(RegisteredUser.Key, applicationSettings.DownloadPaths.First(), string.Empty));

        return Ok(result);
    }
}

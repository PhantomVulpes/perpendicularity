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

    [HttpGet("{rootDirectory}")]
    [HttpGet("{rootDirectory}/{**remainingPath}")]
    public async Task<ActionResult<IEnumerable<string>>> GetStuff(string rootDirectory, string? remainingPath = "")
    {
        var decodedPath = string.IsNullOrEmpty(remainingPath) ? string.Empty : Uri.UnescapeDataString(remainingPath);

        // TODO: Query to get aliases from application settings.
        var applicationSettings = await mediator.RequestResponseAsync<GetApplicationSettingsQuery, ApplicationSettings>(new(RegisteredUser.Key));

        var query = new GetDirectoryContentsQuery(RegisteredUser.Key, applicationSettings.DownloadPaths.Single(config => config.Alias == rootDirectory), decodedPath);
        var result = await mediator.RequestResponseAsync<GetDirectoryContentsQuery, IEnumerable<string>>(query);

        return Ok(result);
    }

    [HttpGet("root-directory")]
    public async Task<ActionResult<IEnumerable<string>>> GetAliases()
    {
        // TODO: Query to get aliases from application settings.
        var applicationSettings = await mediator.RequestResponseAsync<GetApplicationSettingsQuery, ApplicationSettings>(new(RegisteredUser.Key));

        return Ok(applicationSettings.DownloadPaths.Select(config => config.Alias));
    }
}

// Okay so how do I want this to work on the UI, that will probably drive the API functionality.
// First, obviously you get all of the alias's. That's going to be it's own endpoint I think.
// From there, I think I want to use the entire uri as the remaining directory.
// So we have "api/file/TV_Shows/The_Boys/Season_01". This would also be reflected in the UI as well, to their url will also be like browse/TV_Shows/The_Boys/Season_01.
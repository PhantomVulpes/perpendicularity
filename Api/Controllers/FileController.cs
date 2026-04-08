using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Api.ResponseModels;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.QueriedModels;
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
    public async Task<ActionResult<DirectoryContentsResponse>> GetDirectoryContents(string rootDirectory, string? remainingPath = "")
    {
        var decodedPath = string.IsNullOrEmpty(remainingPath) ? string.Empty : Uri.UnescapeDataString(remainingPath);

        var desiredRoot = (await mediator.RequestResponseAsync<GetDirectoryConfigurationsQuery, IEnumerable<DirectoryConfiguration>>(new(RegisteredUser.Key)))
            .Single(config => config.Alias == rootDirectory)
            ;

        var query = new GetDirectoryContentsQuery(RegisteredUser.Key, desiredRoot, decodedPath);
        var contents = await mediator.RequestResponseAsync<GetDirectoryContentsQuery, DirectoryContents>(query);

        return Ok(DirectoryContentsResponse.FromDirectoryContents(contents));
    }

    [HttpGet("root-directory")]
    public async Task<ActionResult<IEnumerable<string>>> GetAliases()
    {
        var result = await mediator.RequestResponseAsync<GetDirectoryConfigurationsQuery, IEnumerable<DirectoryConfiguration>>(new(RegisteredUser.Key));
        return Ok(result.Select(config => config.Alias));
    }
}
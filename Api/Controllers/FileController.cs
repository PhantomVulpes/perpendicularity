using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Api.RequestModels;
using Vulpes.Perpendicularity.Api.ResponseModels;
using Vulpes.Perpendicularity.Api.Services;
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

    [HttpGet("download/{rootDirectory}/{**filePath}")]
    public async Task<IActionResult> DownloadFile(string rootDirectory, string filePath)
    {
        var decodedPath = Uri.UnescapeDataString(filePath);

        var desiredRoot = (await mediator.RequestResponseAsync<GetDirectoryConfigurationsQuery, IEnumerable<DirectoryConfiguration>>(new(RegisteredUser.Key)))
            .Single(config => config.Alias == rootDirectory);

        var query = new GetFileForDownloadQuery(RegisteredUser.Key, desiredRoot, decodedPath);
        var fileInfo = await mediator.RequestResponseAsync<GetFileForDownloadQuery, FileForDownload>(query);

        var fileStream = System.IO.File.OpenRead(fileInfo.FullPath);
        return File(fileStream, "application/octet-stream", fileInfo.FileName);
    }

    [HttpPost("download-zip")]
    public async Task<IActionResult> DownloadFilesAsZip([FromBody] DownloadFilesAsZipRequest request)
    {
        if (request.FilePaths == null || !request.FilePaths.Any())
        {
            return BadRequest("At least one file path must be provided.");
        }

        var desiredRoot = (await mediator.RequestResponseAsync<GetDirectoryConfigurationsQuery, IEnumerable<DirectoryConfiguration>>(new(RegisteredUser.Key)))
            .Single(config => config.Alias == request.RootDirectory);

        var query = new GetFilesAsZipQuery(RegisteredUser.Key, desiredRoot, request.FilePaths);
        var zipInfo = await mediator.RequestResponseAsync<GetFilesAsZipQuery, ZipFileForDownload>(query);
        // Use TempFileStream to automatically delete the temp file after the response is sent
        var fileStream = new TempFileStream(zipInfo.TempFilePath, FileMode.Open, FileAccess.Read, FileShare.None);

        // Use a callback to delete the temp file after the response is sent
        return File(fileStream, "application/zip", zipInfo.FileName, enableRangeProcessing: false);
    }
}
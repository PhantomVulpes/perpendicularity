using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Api.RequestModels;
using Vulpes.Perpendicularity.Api.ResponseModels;
using Vulpes.Perpendicularity.Api.Services;
using Vulpes.Perpendicularity.Core.Commands;
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

        var desiredRoot = (await mediator.RequestResponseAsync(new GetDownloadDirectoryConfigurationsQuery(RegisteredUser.Key)))
            .Single(config => config.Alias == rootDirectory)
            ;

        var query = new GetDirectoryContentsQuery(RegisteredUser.Key, desiredRoot, decodedPath);
        var contents = await mediator.RequestResponseAsync(query);

        return Ok(DirectoryContentsResponse.FromDirectoryContents(contents));
    }

    [HttpGet("download-directory")]
    public async Task<ActionResult<IEnumerable<string>>> GetDownloadAliases()
    {
        var result = await mediator.RequestResponseAsync(new GetDownloadDirectoryConfigurationsQuery(RegisteredUser.Key));
        return Ok(result.Select(config => config.Alias));
    }

    [HttpGet("download/{rootDirectory}/{**filePath}")]
    public async Task<IActionResult> DownloadFile(string rootDirectory, string filePath)
    {
        var decodedPath = Uri.UnescapeDataString(filePath);

        var desiredRoot = (await mediator.RequestResponseAsync(new GetDownloadDirectoryConfigurationsQuery(RegisteredUser.Key)))
            .Single(config => config.Alias == rootDirectory);

        var query = new GetFileForDownloadQuery(RegisteredUser.Key, desiredRoot, decodedPath);
        var fileInfo = await mediator.RequestResponseAsync(query);

        await mediator.ExecuteCommandAsync(new AddUserDownloadCommand(RegisteredUser.Key, [DownloadMetric.Default with { Path = Path.Combine(desiredRoot.Path, decodedPath), SizeBytes = fileInfo.FileSize }]));

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

        var desiredRoot = (await mediator.RequestResponseAsync(new GetDownloadDirectoryConfigurationsQuery(RegisteredUser.Key)))
            .Single(config => config.Alias == request.RootDirectory);

        var query = new GetFilesAsZipQuery(RegisteredUser.Key, desiredRoot, request.FilePaths);
        var zipInfo = await mediator.RequestResponseAsync(query);

        // Record each file for download in the user's metrics.
        var downloadMetrics = request.FilePaths.Select(relativePath =>
        {
            var fullPath = Path.Combine(desiredRoot.Path, relativePath);
            var fileInfo = new FileInfo(fullPath);
            return DownloadMetric.Default with { Path = fullPath, SizeBytes = fileInfo.Length };
        });

        await mediator.ExecuteCommandAsync(new AddUserDownloadCommand(RegisteredUser.Key, downloadMetrics));

        // Use TempFileStream to automatically delete the temp file after the response is sent
        var fileStream = new TempFileStream(zipInfo.TempFilePath, FileMode.Open, FileAccess.Read, FileShare.None);

        // Use a callback to delete the temp file after the response is sent
        return File(fileStream, "application/zip", zipInfo.FileName, enableRangeProcessing: false);
    }

    [HttpGet("upload-directory")]
    public async Task<ActionResult<IEnumerable<string>>> GetUploadAliases()
    {
        var result = await mediator.RequestResponseAsync(new GetUploadDirectoryConfigurationsQuery(RegisteredUser.Key));
        return Ok(result.Select(config => config.Alias));
    }

    [HttpPost("upload/{rootDirectory}")]
    [HttpPost("upload/{rootDirectory}/{**relativePath}")]
    public async Task<IActionResult> UploadFile(string rootDirectory, IFormFile file, string? relativePath = "")
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided or file is empty.");
        }

        var decodedPath = string.IsNullOrEmpty(relativePath) ? string.Empty : Uri.UnescapeDataString(relativePath);

        var desiredRoot = (await mediator.RequestResponseAsync(new GetUploadDirectoryConfigurationsQuery(RegisteredUser.Key)))
            .Single(config => config.Alias == rootDirectory);

        // Combine the relative path with the filename
        var fullRelativePath = string.IsNullOrEmpty(decodedPath)
            ? file.FileName
            : Path.Combine(decodedPath, file.FileName);

        using (var stream = file.OpenReadStream())
        {
            var command = new UploadFileCommand(
                RegisteredUser.Key,
                desiredRoot,
                fullRelativePath,
                stream,
                file.FileName,
                file.Length
            );

            await mediator.ExecuteCommandAsync(command);
        }

        return Ok(new { message = "File uploaded successfully", fileName = file.FileName, size = file.Length });
    }
}
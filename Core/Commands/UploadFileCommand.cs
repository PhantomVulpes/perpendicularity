using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record UploadFileCommand(
    Guid AuthenticatedUserKey,
    DirectoryConfiguration DestinationDirectory,
    string RelativeFilePath,
    Stream FileStream,
    string OriginalFileName,
    long FileSizeBytes
) : Command;

public class UploadFileCommandHandler : CommandHandler<UploadFileCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<ApplicationSettings> settingsRepository;

    public UploadFileCommandHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<ApplicationSettings> settingsRepository)
    {
        this.userRepository = userRepository;
        this.settingsRepository = settingsRepository;
    }

    protected override async Task InternalExecuteAsync(UploadFileCommand command)
    {
        var (normalizedFullPath, normalizedRootPath) = GetPaths(command.RelativeFilePath, command.DestinationDirectory.Path);

        // Ensure the directory exists
        var directoryPath = Path.GetDirectoryName(normalizedFullPath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Save the file
        using (var fileStream = new FileStream(normalizedFullPath, FileMode.Create, FileAccess.Write))
        {
            await command.FileStream.CopyToAsync(fileStream);
        }

        // Record the upload metric
        var uploadMetric = UploadMetric.Default with
        {
            Path = normalizedFullPath,
            SizeBytes = command.FileSizeBytes,
            UploadDate = DateTime.UtcNow
        };

        var user = await userRepository.GetAsync(command.AuthenticatedUserKey);
        var updatedUser = user with
        {
            UploadMetrics = user.UploadMetrics.Append(uploadMetric)
        };

        await userRepository.SaveAsync(updatedUser.PrepareForSave());
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(UploadFileCommand command)
    {
        var user = await userRepository.GetAsync(command.AuthenticatedUserKey);
        if (user.Status != UserStatus.Admin && user.Status != UserStatus.Approved)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} does not have access to upload files.");
        }

        // Check the destination directory is actually allowed
        var settings = await settingsRepository.GetAsync(ApplicationSettings.GlobalApplicationSettingsKey);
        if (!settings.UploadPaths.Select(value => value.Path).Contains(command.DestinationDirectory.Path))
        {
            return AccessResult.Fail($"Destination directory {command.DestinationDirectory.Path} is not an allowed upload path.");
        }

        // Security check: Ensure the resolved path is still within the allowed destination directory
        var (normalizedFullPath, normalizedRootPath) = GetPaths(command.RelativeFilePath, command.DestinationDirectory.Path);

        if (!normalizedFullPath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
        {
            return AccessResult.Fail("Attempted path traversal attack detected.");
        }

        return AccessResult.Success();
    }

    private static (string normalizedFullPath, string normalizedRootPath) GetPaths(string relativeFilePath, string rootDirectoryPath)
    {
        var fullFilePath = Path.Combine(rootDirectoryPath, relativeFilePath);

        var normalizedFullPath = Path.GetFullPath(fullFilePath);
        var normalizedRootPath = Path.GetFullPath(rootDirectoryPath);
        return (normalizedFullPath, normalizedRootPath);
    }
}

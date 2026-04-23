using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record AddExternalProjectCommand(Guid Key, string ProjectName, string ProjectUri, string Tooltip, Guid UserKey) : Command;
public class AddExternalProjectCommandHandler : CommandHandler<AddExternalProjectCommand>
{
    private readonly IModelRepository<ExternalProject> externalProjectRepository;
    private readonly IModelRepository<RegisteredUser> userRepository;

    public AddExternalProjectCommandHandler(IModelRepository<ExternalProject> externalProjectRepository, IModelRepository<RegisteredUser> userRepository)
    {
        this.externalProjectRepository = externalProjectRepository;
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(AddExternalProjectCommand command)
    {
        var newExternalProject = ExternalProject.Default with
        {
            Key = command.Key,
            ProjectName = command.ProjectName,
            ProjectUri = command.ProjectUri,
            Tooltip = command.Tooltip,
        };

        await externalProjectRepository.InsertAsync(newExternalProject.PrepareForInsert());
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(AddExternalProjectCommand command)
    {
        // Only admins can add external projects.
        var user = await userRepository.GetAsync(command.UserKey);

        return user.Status == UserStatus.Admin
            ? AccessResult.Success()
            : AccessResult.Fail($"User {user.ToLogName()} does not have access to {nameof(AddExternalProjectCommand)}.");
    }
}

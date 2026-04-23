using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record DeleteExternalProjectCommand(Guid ProjectKey, Guid UserKey) : Command;
public class DeleteExternalProjectCommandHandler : CommandHandler<DeleteExternalProjectCommand>
{
    private readonly IModelRepository<ExternalProject> externalProjectRepository;
    private readonly IModelRepository<RegisteredUser> userRepository;

    public DeleteExternalProjectCommandHandler(IModelRepository<ExternalProject> externalProjectRepository, IModelRepository<RegisteredUser> userRepository)
    {
        this.externalProjectRepository = externalProjectRepository;
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(DeleteExternalProjectCommand command)
    {
        var projectToDelete = await externalProjectRepository.GetAsync(command.ProjectKey);
        await externalProjectRepository.DeleteAsync(projectToDelete);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(DeleteExternalProjectCommand command)
    {
        // Only admins can delete external projects.
        var user = await userRepository.GetAsync(command.UserKey);

        return user.Status == UserStatus.Admin
            ? AccessResult.Success()
            : AccessResult.Fail($"User {user.ToLogName()} does not have access to {nameof(DeleteExternalProjectCommand)}.");
    }
}

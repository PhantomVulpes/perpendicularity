using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record DeleteExternalProjectRequest
{
    public Guid ProjectKey { get; init; } = Guid.Empty;

    public DeleteExternalProjectCommand ToCommand(RegisteredUser authenticatedUser) =>
        new(ProjectKey, authenticatedUser.Key);
}

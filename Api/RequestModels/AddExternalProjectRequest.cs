using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record AddExternalProjectRequest
{
    public string ProjectName { get; init; } = string.Empty;
    public string ProjectUri { get; init; } = string.Empty;
    public string Tooltip { get; init; } = string.Empty;

    public AddExternalProjectCommand ToCommand(RegisteredUser authenticatedUser) =>
        new(Guid.NewGuid(), ProjectName, ProjectUri, Tooltip, authenticatedUser.Key);
}

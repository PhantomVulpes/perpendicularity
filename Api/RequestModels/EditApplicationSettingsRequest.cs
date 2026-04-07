using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.RequestModels;

public record EditApplicationSettingsRequest(IEnumerable<DirectoryConfiguration> DirectoryConfigurations)
{
    public EditApplicationSettingsCommand ToCommand(RegisteredUser user) => new(DirectoryConfigurations, user.Key);
}
using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.DomainExtensions;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Commands;

public record AddUserUploadCommand(Guid UserKey, IEnumerable<UploadMetric> UploadMetrics) : Command;
public class AddUserUploadCommandHandler : CommandHandler<AddUserUploadCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;

    public AddUserUploadCommandHandler(IModelRepository<RegisteredUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(AddUserUploadCommand command)
    {
        var updatedUser = (await userRepository.GetAsync(command.UserKey)).WithUploadMetric(command.UploadMetrics);

        await userRepository.SaveAsync(updatedUser.PrepareForSave());
    }

    protected override Task<AccessResult> InternalValidateAccessAsync(AddUserUploadCommand command) => AccessResult.Success().FromResult();
}

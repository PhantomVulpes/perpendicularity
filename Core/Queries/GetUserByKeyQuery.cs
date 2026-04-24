using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetUserByKeyQuery(Guid AuthenticatedUserKey, Guid RequestedUserKey) : Query<RegisteredUser>;
public class GetUserByKeyQueryHandler : QueryHandler<GetUserByKeyQuery, RegisteredUser>
{
    private readonly IModelRepository<RegisteredUser> userRepository;

    public GetUserByKeyQueryHandler(IModelRepository<RegisteredUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    protected override async Task<RegisteredUser> InternalRequestAsync(GetUserByKeyQuery query)
    {
        return await userRepository.GetAsync(query.RequestedUserKey);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(GetUserByKeyQuery query)
    {
        var authenticatedUser = await userRepository.GetAsync(query.AuthenticatedUserKey);
        var isRequestingSelf = query.RequestedUserKey == query.AuthenticatedUserKey;
        var isAdmin = authenticatedUser.Status == UserStatus.Admin;

        // User can view their own profile, or admin can view anyone
        if (!isRequestingSelf && !isAdmin)
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} is not authorized to view another user's profile.");
        }

        return AccessResult.Success();
    }
}

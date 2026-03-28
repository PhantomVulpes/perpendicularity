using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetAllUsersQuery(Guid AuthenticatedUserKey) : Query;
public class GetAllUsersQueryHandler : QueryHandler<GetAllUsersQuery, IQueryable<RegisteredUser>>
{
    private readonly IQueryProvider<RegisteredUser> userQueryProvider;
    private readonly IModelRepository<RegisteredUser> userRepository;

    public GetAllUsersQueryHandler(IQueryProvider<RegisteredUser> userQueryProvider, IModelRepository<RegisteredUser> userRepository)
    {
        this.userQueryProvider = userQueryProvider;
        this.userRepository = userRepository;
    }

    protected override async Task<IQueryable<RegisteredUser>> InternalRequestAsync(GetAllUsersQuery query)
    {
        var authenticatedUser = await userRepository.GetAsync(query.AuthenticatedUserKey);
        if (authenticatedUser.Status != UserStatus.Admin)
        {
            // There's no access check on queries (should be added to Electrum), so we gotta do this.
            AccessResult.Fail($"{nameof(RegisteredUser)} does not have ${nameof(UserStatus.Admin)} access.").ThrowIfAccessDenied();
        }

        return await userQueryProvider.BeginQueryAsync();
    }
}
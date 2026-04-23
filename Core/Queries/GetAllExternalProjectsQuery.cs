using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetAllExternalProjectsQuery(Guid AuthenticatedUserKey) : Query<IQueryable<ExternalProject>>;
public class GetAllExternalProjectsQueryHandler : QueryHandler<GetAllExternalProjectsQuery, IQueryable<ExternalProject>>
{
    private readonly IQueryProvider<ExternalProject> externalProjectQueryProvider;
    private readonly IModelRepository<RegisteredUser> userRepository;

    public GetAllExternalProjectsQueryHandler(IQueryProvider<ExternalProject> externalProjectQueryProvider, IModelRepository<RegisteredUser> userRepository)
    {
        this.externalProjectQueryProvider = externalProjectQueryProvider;
        this.userRepository = userRepository;
    }

    protected override async Task<IQueryable<ExternalProject>> InternalRequestAsync(GetAllExternalProjectsQuery query) => await externalProjectQueryProvider.BeginQueryAsync();

    protected override Task<AccessResult> InternalValidateAccessAsync(GetAllExternalProjectsQuery query) => AccessResult.Success().FromResult();
}

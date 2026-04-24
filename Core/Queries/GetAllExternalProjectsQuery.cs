using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetAllExternalProjectsQuery() : Query<IQueryable<ExternalProject>>;
public class GetAllExternalProjectsQueryHandler : QueryHandler<GetAllExternalProjectsQuery, IQueryable<ExternalProject>>
{
    private readonly IQueryProvider<ExternalProject> externalProjectQueryProvider;

    public GetAllExternalProjectsQueryHandler(IQueryProvider<ExternalProject> externalProjectQueryProvider)
    {
        this.externalProjectQueryProvider = externalProjectQueryProvider;
    }

    protected override async Task<IQueryable<ExternalProject>> InternalRequestAsync(GetAllExternalProjectsQuery query) => await externalProjectQueryProvider.BeginQueryAsync();

    // Allow all users, including anonymous users, to access this query
    protected override Task<AccessResult> InternalValidateAccessAsync(GetAllExternalProjectsQuery query) => AccessResult.Success().FromResult();
}

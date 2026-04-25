using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Models;

namespace Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;

public class TestQueryProvider<TAggregateRoot> : IQueryProvider<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    public List<TAggregateRoot> Response { get; set; } = [];

    public Task<IQueryable<TAggregateRoot>> BeginQueryAsync() => Response.AsQueryable().FromResult();
}

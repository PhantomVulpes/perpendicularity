using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Data;

public interface IModelRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    Task<TAggregateRoot> GetAsync(Guid key);
    Task SaveAsync(string editingToken, TAggregateRoot record);
    Task DeleteAsync(TAggregateRoot record);
    Task InsertAsync(TAggregateRoot record);
}

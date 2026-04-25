using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Models;
using Vulpes.Electrum.Domain.Validation;

namespace Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Data;


public class TestModelRepository<TAggregateRoot> : IModelRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    public Dictionary<Guid, TAggregateRoot> Entries { get; init; } = [];
    public List<TAggregateRoot> DeletedEntries { get; init; } = [];
    public List<TAggregateRoot> InsertedEntries { get; init; } = [];
    public List<TAggregateRoot> SavedEntries { get; init; } = [];

    public void AddEntryForTest(TAggregateRoot entry) => Entries.Add(entry.Key, entry);

    public Task DeleteAsync(TAggregateRoot record)
    {
        Entries.Remove(record.Key);
        DeletedEntries.Add(record);

        return Task.CompletedTask;
    }

    public Task<TAggregateRoot> GetAsync(Guid key) => Entries[key].FromResult();
    public Task InsertAsync(InsertModel<TAggregateRoot> insertRecord)
    {
        var entry = insertRecord.Value;
        Entries.Add(entry.Key, entry);
        InsertedEntries.Add(entry);

        return Task.CompletedTask;
    }
    public Task SaveAsync(SaveModel<TAggregateRoot> saveRecord)
    {
        var entry = saveRecord.Value;
        Entries[entry.Key] = entry;
        SavedEntries.Add(entry);

        return Task.CompletedTask;
    }
}

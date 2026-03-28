using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Logging;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Infrastructure.Mongo;

public class MongoRepository<TAggregateRoot> : IModelRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    private readonly IMongoProvider mongoProvider;
    private readonly ILogger<MongoRepository<TAggregateRoot>> logger;

    public MongoRepository(IMongoProvider mongoProvider, ILogger<MongoRepository<TAggregateRoot>> logger)
    {
        this.mongoProvider = mongoProvider;
        this.logger = logger;
    }

    public async Task DeleteAsync(TAggregateRoot record)
    {
        try
        {
            var deleteResult = await mongoProvider.GetCollection<TAggregateRoot>(CqrsType.Command)
                .DeleteOneAsync(
                value => value.Key.Equals(record.Key));

            logger.LogInformation($"{LogTags.EntityDeleted} Successfully deleted {typeof(TAggregateRoot).Name}, {record.ToLogName()}.");
        }
        catch (Exception ex)
        {
            logger.LogError($"{LogTags.Failure} Failed to delete {typeof(TAggregateRoot).Name}, {record.ToLogName()}: {ex.Message}");
        }
    }

    public Task<TAggregateRoot> GetAsync(Guid key)
    {
        var result = mongoProvider.GetQuery<TAggregateRoot>(CqrsType.Query).Where(record => record.Key.Equals(key)).FirstOrPerhaps();

        return result.ElseThrow($"Could not find object {typeof(TAggregateRoot).Name} with key {key}.").FromResult();
    }

    public async Task InsertAsync(TAggregateRoot record)
    {
        // TODO: Try catch around this.
        var collection = mongoProvider.GetCollection<TAggregateRoot>(CqrsType.Command);
        await collection.InsertOneAsync(record);

        logger.LogDebug($"{LogTags.EntityInserted} Inserted object {typeof(TAggregateRoot).Name}, {record.ToLogName()}.");
    }

    public async Task SaveAsync(string editingToken, TAggregateRoot record)
    {
        // TODO: Try catch around these and finish implementation.
        var oldEditingToken = record.EditingToken;

        if (string.IsNullOrEmpty(oldEditingToken))
        {
            // Throw EmptyEditingToken
        }

        if (editingToken == oldEditingToken)
        {
            // Throw StaleEditingToken
        }

        var saveResult = await mongoProvider
            .GetCollection<TAggregateRoot>(CqrsType.Command)
            .ReplaceOneAsync(value => value.Key == record.Key && value.EditingToken == oldEditingToken, record)
            ;

        if (saveResult.ModifiedCount <= 0)
        {
            // Throw ConcurrentModification
        }

        logger.LogDebug($"{LogTags.EntityUpdated} Updated object {typeof(TAggregateRoot).Name}, {record.ToLogName()}.");
    }
}
using MongoDB.Driver;
using Vulpes.Perpendicularity.Core.Data;

namespace Vulpes.Perpendicularity.Infrastructure.Mongo;

public abstract class MongoIndexDefinition<TModel> : IIndexDefinition
{
    private readonly IMongoProvider mongoProvider;

    protected MongoIndexDefinition(IMongoProvider mongoProvider)
    {
        this.mongoProvider = mongoProvider;
    }

    public abstract string Name { get; }
    public virtual async Task<bool> ExistsAsync()
    {
        var collection = GetCollection();
        var indexList = await collection.Indexes.ListAsync();

        while (await indexList.MoveNextAsync())
        {
            var currentBatch = indexList.Current;
            var found = currentBatch.Select(bson => bson.GetValue("name").AsString).Any(name => name == Name);

            if (found)
            {
                return true;
            }
        }

        return false;
    }

    public Task CreateIndexAsync() => CreateIndexInternalAsync();
    protected abstract Task CreateIndexInternalAsync();

    protected IMongoCollection<TModel> GetCollection() => mongoProvider.GetCollection<TModel>(CqrsType.Command);
}
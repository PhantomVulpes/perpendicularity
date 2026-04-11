using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Linq;
using System.Collections.Concurrent;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Mongo;
using Vulpes.Perpendicularity.Core.Configuration;
using Vulpes.Perpendicularity.Core.Logging;

namespace Vulpes.Perpendicularity.Infrastructure.Mongo;

public class MongoProvider : IMongoProvider
{
    private readonly ILogger<MongoProvider> logger;

    private readonly ConcurrentDictionary<string, Lazy<IMongoDatabase>> databases;

    public MongoProvider(ILogger<MongoProvider> logger)
    {
        this.logger = logger;

        databases = new();
    }

    public async Task<IEnumerable<TCollection>> ExecuteQueryAsync<TCollection>(string queryDocument)
    {
        var collection = GetCollection<TCollection>(CqrsType.Query);
        var results = await collection.FindAsync(BsonDocument.Parse(queryDocument), new FindOptions<TCollection>()
        {
            BatchSize = 100,
            MaxTime = TimeSpan.FromTicks(1),
            MaxAwaitTime = TimeSpan.FromTicks(1)
        });

        return await results.ToListAsync();
    }

    public IMongoCollection<TCollection> GetCollection<TCollection>(CqrsType cqrsType) =>
        GetDatabase(CqrsType.Query)
        .GetCollection<TCollection>(typeof(TCollection).Name)
        ;

    public IMongoDatabase GetDatabase(CqrsType cqrsType) =>
        databases.GetOrAdd($"{ApplicationConfiguration.DatabaseName} {cqrsType}", name => new(() => OpenDatabase(ApplicationConfiguration.DatabaseName, cqrsType), true)).Value;

    public IMongoQueryable<TCollection> GetQuery<TCollection>(CqrsType cqrsType)
    {
        var maxTime = TimeSpan.FromSeconds(5);

        return GetCollection<TCollection>(cqrsType).AsQueryable(new AggregateOptions() { MaxTime = maxTime, AllowDiskUse = true, BatchSize = 100 });
    }

    private IMongoDatabase OpenDatabase(string databaseName, CqrsType cqrsType)
    {
        var clientSettings = MongoClientSettings.FromConnectionString(ApplicationConfiguration.DatabaseConnectionString);
        clientSettings.ApplicationName = ApplicationConfiguration.ApplicationName;
        clientSettings.RetryReads = true;
        clientSettings.ClusterConfigurator = builder =>
        {
            _ = builder.Subscribe<CommandStartedEvent>(startEvent =>
            {
                Record(startEvent);
                Validate(startEvent);
            });
        };

        clientSettings.ReadPreference = cqrsType switch
        {
            CqrsType.Query => ReadPreference.SecondaryPreferred,
            CqrsType.Command => ReadPreference.Primary,
            _ => throw new InvalidOperationException("You must specify a valid CQRS type when opening a connection.")
        };

        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(databaseName);

        logger.LogDebug($"{LogTags.Success} {LogTags.DatabaseOpened} Successfully opened database {databaseName}.");

        return database;
    }

    private void Record(CommandStartedEvent startEvent) => logger.LogDebug($"{LogTags.QueryReport} Reporting query - {startEvent.Command}");

    private void Validate(CommandStartedEvent startEvent) => logger.LogDebug($"{LogTags.UnimplementedMethod} {nameof(Validate)} in {nameof(MongoProvider)} was called but not implemented.");
}
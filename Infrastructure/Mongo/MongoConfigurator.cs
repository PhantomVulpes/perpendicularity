using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Infrastructure.Mongo;

public static class MongoConfigurator
{
    public static void Configure()
    {
        var pack = new ConventionPack()
        {
            new ConfigureToStringConvention(typeInfo => typeInfo.IsEnum),
            new ConfigureToStringConvention(typeInfo => typeInfo == typeof(Guid)),
        };

        ConventionRegistry.Register("Global MongoDB Conventions", pack, t => true);

        _ = BsonClassMap.RegisterClassMap<AggregateRoot>(cm =>
        {
            cm.AutoMap();
            _ = cm.MapIdMember(c => c.Key).SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.GuidGenerator.Instance);
        });
    }
}
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.ValueObjects;
using Vulpes.Perpendicularity.Infrastructure.Mongo.Conventions;
using Vulpes.Perpendicularity.Infrastructure.Mongo.Serialization;

namespace Vulpes.Perpendicularity.Infrastructure.Mongo;

public static class MongoConfigurator
{
    public static void Configure()
    {
        var pack = new ConventionPack()
        {
            new ConfigureToStringConvention(typeInfo => typeInfo.IsEnum),
            new ConfigureToStringConvention(typeInfo => typeInfo == typeof(Guid))
        };

        ConventionRegistry.Register("Global MongoDB Conventions", pack, t => true);

        // Register custom serialization provider for ValueObjectBase types
        BsonSerializer.RegisterSerializationProvider(new ValueObjectSerializationProvider());

        _ = BsonClassMap.RegisterClassMap<AggregateRoot>(cm =>
        {
            cm.AutoMap();
            _ = cm.MapIdMember(c => c.Key).SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.GuidGenerator.Instance);
        });
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;

namespace Vulpes.Perpendicularity.Infrastructure.Mongo;

internal class ConfigureToStringConvention : ConventionBase, IMemberMapConvention
{
    private readonly Func<TypeInfo, bool> predicate;

    public ConfigureToStringConvention(Func<TypeInfo, bool> predicate)
    {
        this.predicate = predicate;
    }

    public void Apply(BsonMemberMap memberMap)
    {
        var memberTypeInfo = memberMap.MemberType.GetTypeInfo();
        if (predicate(memberTypeInfo))
        {
            var serializer = memberMap.GetSerializer();
            if (serializer is IRepresentationConfigurable representationConfigurableSerializer)
            {
                var representation = BsonType.String;
                var reconfiguredSerializer = representationConfigurableSerializer.WithRepresentation(representation);
                _ = memberMap.SetSerializer(reconfiguredSerializer);
            }
        }
    }
}
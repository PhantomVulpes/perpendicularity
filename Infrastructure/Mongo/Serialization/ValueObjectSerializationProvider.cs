using MongoDB.Bson.Serialization;
using Vulpes.Perpendicularity.Core.ValueObjects;

namespace Vulpes.Perpendicularity.Infrastructure.Mongo.Serialization;

internal class ValueObjectSerializationProvider : IBsonSerializationProvider
{
    public IBsonSerializer? GetSerializer(Type type)
    {
        // Check if the type inherits from ValueObjectBase
        if (type.IsAssignableTo(typeof(ValueObjectBase)) && type != typeof(ValueObjectBase))
        {
            // Create the generic serializer type for this specific type
            var serializerType = typeof(ValueObjectSerializer<>).MakeGenericType(type);
            return (IBsonSerializer)Activator.CreateInstance(serializerType)!;
        }

        return null;
    }
}

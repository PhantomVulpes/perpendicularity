using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Vulpes.Perpendicularity.Core.ValueObjects;

namespace Vulpes.Perpendicularity.Infrastructure.Mongo.Serialization;

public class ValueObjectSerializer<T> : SerializerBase<T> where T : ValueObjectBase
{
    public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonReader = context.Reader;
        var stringValue = bsonReader.ReadString();

        // Uses the constructor to create the instance
        return (T)Activator.CreateInstance(typeof(T), stringValue)!;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        context.Writer.WriteString(value.Value);
    }
}
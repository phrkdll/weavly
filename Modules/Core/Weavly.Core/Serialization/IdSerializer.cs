using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Serialization;

public sealed class IdSerializer<T> : StructSerializerBase<T>
    where T : struct, IWeavlyId
{
    private static readonly ConstructorInfo Constructor =
        typeof(T).GetConstructor([typeof(string)])
        ?? throw new InvalidOperationException($"{typeof(T).Name} must have a valid constructor.");

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        context.Writer.WriteObjectId(ObjectId.Parse(value.Value));
    }

    public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var objectId = context.Reader.ReadObjectId();
        return (T)Constructor.Invoke([objectId.ToString()]);
    }
}

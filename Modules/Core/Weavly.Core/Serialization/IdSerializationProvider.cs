using MongoDB.Bson.Serialization;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Serialization;

public sealed class IdSerializationProvider : IBsonSerializationProvider
{
    public IBsonSerializer? GetSerializer(Type type)
    {
        if (!type.IsValueType)
        {
            return null;
        }

        if (!typeof(IWeavlyId).IsAssignableFrom(type))
        {
            return null;
        }

        var serializerType = typeof(IdSerializer<>).MakeGenericType(type);
        return (IBsonSerializer)Activator.CreateInstance(serializerType)!;
    }
}

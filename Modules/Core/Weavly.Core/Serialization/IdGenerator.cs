using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Serialization;

public sealed class IdGenerator<T> : IIdGenerator
    where T : struct, IWeavlyId
{
    public object GenerateId(object container, object document)
    {
        return Activator.CreateInstance(typeof(T), ObjectId.GenerateNewId().ToString())!;
    }

    public bool IsEmpty(object? id)
    {
        return id is null || id.Equals(default(T));
    }
}

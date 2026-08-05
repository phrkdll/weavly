using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Models;

namespace Weavly.Core.Serialization;

public sealed class WeavlyIdConvention : IMemberMapConvention
{
    public string Name => nameof(WeavlyIdConvention);

    public void Apply(BsonMemberMap memberMap)
    {
        if (!typeof(IWeavlyId).IsAssignableFrom(memberMap.MemberType) || memberMap.MemberName != nameof(Document<>.Id))
        {
            return;
        }

        var generatorType = typeof(IdGenerator<>).MakeGenericType(memberMap.MemberType);

        var generator = (IIdGenerator)Activator.CreateInstance(generatorType)!;
        memberMap.SetIdGenerator(generator);
    }
}

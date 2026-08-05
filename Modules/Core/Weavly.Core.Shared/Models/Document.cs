using MongoDB.Bson.Serialization.Attributes;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Shared.Models;

public abstract record Document<TDocumentId>
    where TDocumentId : struct, IWeavlyId
{
    [BsonId]
    public TDocumentId Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? TouchedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}

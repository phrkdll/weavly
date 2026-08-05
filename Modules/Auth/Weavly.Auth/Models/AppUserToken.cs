using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Weavly.Auth.Enums;

namespace Weavly.Auth.Models;

public sealed record AppUserToken(AppUserTokenPurpose Purpose, DateTime? ExpiresAt = null)
{
    [BsonRepresentation(BsonType.String)]
    public Guid Value { get; init; } = Guid.NewGuid();
}

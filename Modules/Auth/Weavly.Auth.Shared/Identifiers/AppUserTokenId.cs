using Strongly;

namespace Weavly.Auth.Shared.Identifiers;

[Strongly(StronglyType.String, StronglyConverter.EfValueConverter)]
public readonly partial record struct AppUserTokenId;

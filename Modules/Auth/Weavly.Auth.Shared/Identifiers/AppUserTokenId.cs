using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Identifiers;

public record struct AppUserTokenId(string Value) : IWeavlyId;

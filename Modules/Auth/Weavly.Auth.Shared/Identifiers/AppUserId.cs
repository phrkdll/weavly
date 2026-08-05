using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Identifiers;

public record struct AppUserId(string Value) : IWeavlyId;

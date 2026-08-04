using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Identifiers;

public record struct AppRoleId(string Value) : IWeavlyId;
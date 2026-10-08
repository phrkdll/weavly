using Weavly.Auth.Shared.Identifiers;

namespace Weavly.Auth.Shared.Features.SeedAppRole;

public sealed record SeedAppRoleResponse(AppRoleId Id, bool Created);

using Weavly.Auth.Shared.Identifiers;

namespace Weavly.Auth.Shared.Features.SeedAppUser;

public sealed record SeedAppUserResponse(AppUserId Id, bool Created);

using Weavly.Auth.Shared.Identifiers;

namespace Weavly.Auth.Shared.Features.Register;

public sealed record RegisterUserResponse(AppUserId? Id)
{
    public static RegisterUserResponse Empty => new(AppUserId.Empty);
}

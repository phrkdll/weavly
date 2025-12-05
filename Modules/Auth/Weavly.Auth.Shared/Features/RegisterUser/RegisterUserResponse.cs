using Weavly.Auth.Shared.Identifiers;

namespace Weavly.Auth.Shared.Features.RegisterUser;

public sealed record RegisterUserResponse(AppUserId? Id)
{
    public static RegisterUserResponse Empty => new(AppUserId.Empty);
}

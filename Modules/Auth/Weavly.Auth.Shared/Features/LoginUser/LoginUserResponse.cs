namespace Weavly.Auth.Shared.Features.LoginUser;

public sealed record LoginUserResponse(string Token, DateTime ExpiresAt)
{
    public static LoginUserResponse Empty => new(string.Empty, default);
}

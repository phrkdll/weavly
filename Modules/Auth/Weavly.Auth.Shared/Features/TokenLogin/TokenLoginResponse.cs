namespace Weavly.Auth.Shared.Features.TokenLogin;

public sealed record TokenLoginResponse(string Token, DateTime ExpiresAt)
{
    public static TokenLoginResponse Empty => new(string.Empty, default);
}

namespace Weavly.Auth.Shared.Features.TwoFactorAuth.Verify;

public sealed record VerifyTwoFactorAuthResponse(string Token, DateTime ExpiresAt)
{
    public static VerifyTwoFactorAuthResponse Empty => new(string.Empty, default);
}

namespace Weavly.Auth.Shared.Features.EnableTwoFactorAuth;

public sealed record EnableTwoFactorAuthResponse(string TextCode, string QrCode);

namespace Weavly.Auth.Shared.Features.TwoFactorAuth.Enable;

public sealed record EnableTwoFactorAuthResponse(string TextCode, string QrCode);

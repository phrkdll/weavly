using FastEndpoints;

namespace Weavly.Auth.Shared.Features.TwoFactorAuth.Verify;

public sealed record VerifyTwoFactorAuthCommand(string Email, string VerificationPin) : ICommand<Result>;

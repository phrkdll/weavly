using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.VerifyTwoFactorAuth;

public sealed record VerifyTwoFactorAuthCommand(string Email, string VerificationPin) : IWeavlyCommand;

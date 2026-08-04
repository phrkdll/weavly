using Weavly.Auth.Features.VerifyTwoFactorAuth;
using Weavly.Auth.Shared.Features.VerifyTwoFactorAuth;

namespace Weavly.Auth.Tests.Features.VerifyTwoFactorAuth;

public sealed class VerifyTwoFactorAuthEndpointTests()
    : AuthEndpointTests<VerifyTwoFactorAuthEndpoint, VerifyTwoFactorAuthCommand, VerifyTwoFactorAuthResponse>(
        new VerifyTwoFactorAuthCommand(string.Empty, string.Empty));
using Weavly.Auth.Features.VerifyTwoFactorAuth;
using Weavly.Auth.Shared.Features.VerifyTwoFactorAuth;

namespace Weavly.Auth.Tests.Features.VerifyTwoFactorAuth;

public sealed class VerifyTwoFactorAuthEndpointTests
    : AuthEndpointTests<VerifyTwoFactorAuthEndpoint, VerifyTwoFactorAuthCommand>
{
    public VerifyTwoFactorAuthEndpointTests()
        : base(new VerifyTwoFactorAuthCommand(string.Empty, string.Empty)) { }
}

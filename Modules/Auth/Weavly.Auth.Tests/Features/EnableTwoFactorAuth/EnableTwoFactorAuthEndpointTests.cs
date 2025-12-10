using Weavly.Auth.Features.EnableTwoFactorAuth;
using Weavly.Auth.Shared.Features.EnableTwoFactorAuth;

namespace Weavly.Auth.Tests.Features.EnableTwoFactorAuth;

public sealed class EnableTwoFactorAuthEndpointTests
    : AuthEndpointTests<EnableTwoFactorAuthEndpoint, EnableTwoFactorAuthCommand>
{
    public EnableTwoFactorAuthEndpointTests()
        : base(new EnableTwoFactorAuthCommand()) { }
}

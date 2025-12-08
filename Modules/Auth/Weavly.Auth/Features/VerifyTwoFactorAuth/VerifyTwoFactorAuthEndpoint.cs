using Weavly.Auth.Shared.Features.VerifyTwoFactorAuth;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.VerifyTwoFactorAuth;

internal sealed class VerifyTwoFactorAuthEndpoint : GetEndpoint<VerifyTwoFactorAuthCommand, AuthModule>
{
    public VerifyTwoFactorAuthEndpoint(IMessageBus bus)
        : base("user/2fa/verify", bus)
    {
        Authorize();
    }
}

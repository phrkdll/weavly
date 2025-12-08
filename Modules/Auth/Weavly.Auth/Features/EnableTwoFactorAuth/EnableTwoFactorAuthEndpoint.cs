using Weavly.Auth.Shared.Features.EnableTwoFactorAuth;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.EnableTwoFactorAuth;

internal sealed class EnableTwoFactorAuthEndpoint : PostEndpoint<EnableTwoFactorAuthCommand, AuthModule>
{
    public EnableTwoFactorAuthEndpoint(IMessageBus bus)
        : base("user/2fa/enable", bus)
    {
        Authorize();
    }
}

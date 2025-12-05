using Weavly.Auth.Shared.Features.EnableTwoFactorAuth;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.EnableTwoFactorAuth;

internal sealed class EnableTwoFactorAuthEndpoint(IMessageBus bus)
    : PostEndpoint<EnableTwoFactorAuthCommand, AuthModule>("user/2fa/enable", bus);

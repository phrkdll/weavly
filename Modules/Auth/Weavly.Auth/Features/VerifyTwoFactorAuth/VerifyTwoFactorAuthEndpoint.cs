using Weavly.Auth.Shared.Features.VerifyTwoFactorAuth;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.VerifyTwoFactorAuth;

internal sealed class VerifyTwoFactorAuthEndpoint(IMessageBus bus)
    : GetEndpoint<VerifyTwoFactorAuthCommand, AuthModule>("user/2fa/verify", bus);

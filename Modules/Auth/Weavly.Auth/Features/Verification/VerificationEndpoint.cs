using Weavly.Auth.Shared.Features.Verification;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.Verification;

public sealed class VerificationEndpoint(IMessageBus bus)
    : GetEndpoint<VerificationCommand, AuthModule>("user/verify", bus);

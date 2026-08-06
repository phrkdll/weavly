using Weavly.Auth.Shared.Features.Verification;
using Weavly.Core.Shared.Implementation.Endpoints;
using Weavly.Core.Shared.Models;
using Wolverine;

namespace Weavly.Auth.Features.Verification;

public sealed class VerificationEndpoint(IMessageBus bus)
    : GetEndpoint<VerificationCommand, EmptyResponse, AuthModule>("user/verify", bus);

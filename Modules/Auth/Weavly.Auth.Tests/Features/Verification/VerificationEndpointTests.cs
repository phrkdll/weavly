using Weavly.Auth.Features.Verification;
using Weavly.Auth.Shared.Features.Verification;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Tests.Features.Verification;

public sealed class VerificationEndpointTests()
    : AuthEndpointTests<VerificationEndpoint, VerificationCommand, EmptyResponse>(
        new VerificationCommand(Guid.Empty)
    );

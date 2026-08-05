using Weavly.Auth.Features.Verification;
using Weavly.Auth.Shared.Features.Verification;

namespace Weavly.Auth.Tests.Features.Verification;

public sealed class VerificationEndpointTests()
    : AuthEndpointTests<VerificationEndpoint, VerificationCommand, VerificationResponse>(
        new VerificationCommand(Guid.Empty)
    );

using Weavly.Auth.Features.Verification;
using Weavly.Auth.Shared.Features.VerifyUser;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Tests.Features.VerifyUser;

public sealed class VerifyUserEndpointTests()
    : AuthEndpointTests<VerifyUserEndpoint, VerifyUserCommand, EmptyResponse>(new VerifyUserCommand(Guid.Empty));

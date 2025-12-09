using Weavly.Auth.Features.TokenLogin;
using Weavly.Auth.Shared.Features.TokenLogin;

namespace Weavly.Auth.Tests.Features.TokenLogin;

public sealed class TokenLoginEndpointTests : AuthEndpointTests<TokenLoginEndpoint, TokenLoginCommand>
{
    public TokenLoginEndpointTests()
        : base(new TokenLoginCommand(Guid.Empty)) { }
}

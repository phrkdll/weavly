using Weavly.Auth.Features.LoginUser;
using Weavly.Auth.Shared.Features.LoginUser;

namespace Weavly.Auth.Tests.Features.LoginUser;

public sealed class LoginUserEndpointTests : AuthEndpointTests<LoginUserEndpoint, LoginUserCommand>
{
    public LoginUserEndpointTests()
        : base(new LoginUserCommand(string.Empty, string.Empty)) { }
}

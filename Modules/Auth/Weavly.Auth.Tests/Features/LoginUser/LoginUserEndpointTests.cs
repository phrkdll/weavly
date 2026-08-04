using Weavly.Auth.Features.LoginUser;
using Weavly.Auth.Shared.Features.LoginUser;

namespace Weavly.Auth.Tests.Features.LoginUser;

public sealed class LoginUserEndpointTests()
    : AuthEndpointTests<LoginUserEndpoint, LoginUserCommand, LoginUserResponse>(new LoginUserCommand(string.Empty, string.Empty));
using Weavly.Auth.Features.RegisterUser;
using Weavly.Auth.Shared.Features.RegisterUser;

namespace Weavly.Auth.Tests.Features.RegisterUser;

public sealed class RegisterUserEndpointTests()
    : AuthEndpointTests<RegisterUserEndpoint, RegisterUserCommand, RegisterUserResponse>(
        new RegisterUserCommand("admin@test.local", "P@ssw0rd!"));
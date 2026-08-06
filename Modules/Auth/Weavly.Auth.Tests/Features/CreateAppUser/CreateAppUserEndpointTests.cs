using Weavly.Auth.Features.CreateAppUser;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Tests.Features.CreateAppUser;

public sealed class CreateAppUserEndpointTests()
    : AuthEndpointTests<CreateAppUserEndpoint, CreateAppUserCommand, CreateAppUserResponse>(
        new CreateAppUserCommand("admin@test.local", "Admin", "Admin", "P@ssw0rd!")
    );
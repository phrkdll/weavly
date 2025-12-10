using Weavly.Auth.Features.CreateAppUser;
using Weavly.Auth.Shared.Features.CreateAppUser;

namespace Weavly.Auth.Tests.Features.CreateAppUser;

public sealed class CreateAppUserEndpointTests : AuthEndpointTests<CreateAppUserEndpoint, CreateAppUserCommand>
{
    public CreateAppUserEndpointTests()
        : base(new CreateAppUserCommand("admin@test.local", "Admin", "Admin", "P@ssw0rd!")) { }
}

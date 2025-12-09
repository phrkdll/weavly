using Weavly.Auth.Features.CreateAppRole;
using Weavly.Auth.Shared.Features.CreateAppRole;

namespace Weavly.Auth.Tests.Features.CreateAppRole;

public sealed class CreateAppRoleEndpointTests : AuthEndpointTests<CreateAppRoleEndpoint, CreateAppRoleCommand>
{
    public CreateAppRoleEndpointTests()
        : base(new CreateAppRoleCommand("Test")) { }
}

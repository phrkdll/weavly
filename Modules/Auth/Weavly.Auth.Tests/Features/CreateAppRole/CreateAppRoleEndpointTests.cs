using Weavly.Auth.Features.CreateAppRole;
using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Tests.Features.CreateAppRole;

public sealed class CreateAppRoleEndpointTests()
    : AuthEndpointTests<CreateAppRoleEndpoint, CreateAppRoleCommand, CreateAppRoleResponse>(
        new CreateAppRoleCommand("Test")
    );

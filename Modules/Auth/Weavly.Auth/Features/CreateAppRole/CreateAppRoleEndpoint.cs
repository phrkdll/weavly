using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.CreateAppRole;

public sealed class CreateAppRoleEndpoint : PostEndpoint<CreateAppRoleCommand, object, AuthModule>
{
    public CreateAppRoleEndpoint(IMessageBus bus)
        : base("role", bus)
    {
        Authorize();
    }
}
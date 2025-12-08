using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.CreateAppRole;

internal sealed class CreateAppRoleEndpoint : PostEndpoint<CreateAppRoleCommand, AuthModule>
{
    public CreateAppRoleEndpoint(IMessageBus bus)
        : base("role", bus)
    {
        Authorize();
    }
}

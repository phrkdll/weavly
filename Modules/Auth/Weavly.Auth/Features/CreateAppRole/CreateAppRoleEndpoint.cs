using Weavly.Auth.Shared.Features.CreateAppRole;

namespace Weavly.Auth.Features.CreateAppRole;

internal sealed class CreateAppRoleEndpoint : Endpoint<CreateAppRoleCommand>
{
    public override void Configure()
    {
        Post("role");
    }

    public override async Task HandleAsync(CreateAppRoleCommand request, CancellationToken ct)
    {
        await HandleDefaultAsync(request, ct);
    }
}

using Weavly.Auth.Shared.Features.CreateAppUser;

namespace Weavly.Auth.Features.CreateAppUser;

internal sealed class CreateAppUserEndpoint : Endpoint<CreateAppUserCommand>
{
    public override void Configure()
    {
        Post("user");
    }

    public override async Task HandleAsync(CreateAppUserCommand request, CancellationToken ct)
    {
        await HandleDefaultAsync(request, ct);
    }
}

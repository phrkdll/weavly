using Weavly.Auth.Shared.Features.TwoFactorAuth.Enable;

namespace Weavly.Auth.Features.TwoFactorAuth.Enable;

internal sealed class EnableTwoFactorAuthEndpoint() : Endpoint<EnableTwoFactorAuthCommand>()
{
    public override void Configure()
    {
        Post("user/2fa/enable");
    }

    public override async Task HandleAsync(EnableTwoFactorAuthCommand request, CancellationToken ct)
    {
        await HandleDefaultAsync(request, ct);
    }
}

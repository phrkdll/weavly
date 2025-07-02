using Weavly.Auth.Shared.Features.TwoFactorAuth.Verify;

namespace Weavly.Auth.Features.TwoFactorAuth.Verify;

internal sealed class VerifyTwoFactorAuthEndpoint() : Endpoint<VerifyTwoFactorAuthCommand>()
{
    public override void Configure()
    {
        Post("user/2fa/verify");
        AllowAnonymous();
    }

    public override async Task HandleAsync(VerifyTwoFactorAuthCommand request, CancellationToken ct)
    {
        await HandleDefaultAsync(request, ct);
    }
}

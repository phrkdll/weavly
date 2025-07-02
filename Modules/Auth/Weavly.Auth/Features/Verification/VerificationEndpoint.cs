using Weavly.Auth.Shared.Features.Verification;

namespace Weavly.Auth.Features.Verification;

public sealed class VerificationEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("user/verify");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var request = new VerificationCommand(Query<Guid>("token"));

        await HandleDefaultAsync(request, ct);
    }
}

using Weavly.Auth.Shared.Features.TokenLogin;

namespace Weavly.Auth.Features.TokenLogin;

internal sealed class TokenLoginEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("user/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var request = new TokenLoginCommand(Query<Guid>("token"));

        await HandleDefaultAsync(request, ct);
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.EnableTwoFactorAuth;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.EnableTwoFactorAuth;

internal sealed class EnableTwoFactorAuthEndpoint(IMessageBus bus) : IWeavlyEndpoint<EnableTwoFactorAuthCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapPost("2fa/enable", () => HandleAsync);

    public async Task<IResult> HandleAsync(EnableTwoFactorAuthCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

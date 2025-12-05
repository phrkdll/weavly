using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.VerifyTwoFactorAuth;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.VerifyTwoFactorAuth;

internal sealed class VerifyTwoFactorAuthEndpoint(IMessageBus bus) : IWeavlyEndpoint<VerifyTwoFactorAuthCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapGet("2fa/verify", () => HandleAsync);

    public async Task<IResult> HandleAsync(VerifyTwoFactorAuthCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

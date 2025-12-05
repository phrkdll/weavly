using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.Verification;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.Verification;

internal sealed class VerificationEndpoint(IMessageBus bus) : IWeavlyEndpoint<VerificationCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapGet("user/verify", () => HandleAsync);

    public async Task<IResult> HandleAsync(VerificationCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

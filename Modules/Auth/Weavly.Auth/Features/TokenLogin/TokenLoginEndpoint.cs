using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.TokenLogin;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.TokenLogin;

internal sealed class TokenLoginEndpoint(IMessageBus bus) : IWeavlyEndpoint<TokenLoginCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapGet("user/login", () => HandleAsync);

    public async Task<IResult> HandleAsync(TokenLoginCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

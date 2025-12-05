using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.LoginUser;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.LoginUser;

internal sealed class LoginUserEndpoint(IMessageBus bus) : IWeavlyEndpoint<LoginUserCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapPost("user/login", () => HandleAsync);

    public async Task<IResult> HandleAsync(LoginUserCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

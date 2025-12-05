using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.RegisterUser;

internal sealed class RegisterUserEndpoint(IMessageBus bus) : IWeavlyEndpoint<RegisterUserCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapPost("user/register", () => HandleAsync);

    public async Task<IResult> HandleAsync(RegisterUserCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

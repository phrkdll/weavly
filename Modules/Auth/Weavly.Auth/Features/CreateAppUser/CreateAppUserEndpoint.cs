using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.CreateAppUser;

internal sealed class CreateAppUserEndpoint(IMessageBus bus) : IWeavlyEndpoint<CreateAppUserCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapPost("user", () => HandleAsync).RequireAuthorization();

    public async Task<IResult> HandleAsync(CreateAppUserCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

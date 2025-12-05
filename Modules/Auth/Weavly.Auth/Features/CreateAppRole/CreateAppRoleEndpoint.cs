using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.CreateAppRole;

internal sealed class CreateAppRoleEndpoint(IMessageBus bus) : IWeavlyEndpoint<CreateAppRoleCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapPost("role", () => HandleAsync).RequireAuthorization();

    public async Task<IResult> HandleAsync(CreateAppRoleCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

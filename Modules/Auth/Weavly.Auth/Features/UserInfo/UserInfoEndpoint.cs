using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Features.UserInfo;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.UserInfo;

internal sealed class UserInfoEndpoint(IMessageBus bus) : IWeavlyEndpoint<UserInfoCommand>
{
    public void MapEndpoint(WebApplication app) => app.MapGet("user/info", () => HandleAsync);

    public async Task<IResult> HandleAsync(UserInfoCommand request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

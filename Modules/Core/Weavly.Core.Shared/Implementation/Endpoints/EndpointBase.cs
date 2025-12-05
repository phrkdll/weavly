using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Implementation.Endpoints;

public abstract class EndpointBase<TRequest, TModule>(IMessageBus bus) : IWeavlyEndpoint<TRequest>
    where TRequest : IWeavlyCommand
    where TModule : IWeavlyModule
{
    public virtual async Task<IResult> HandleAsync(TRequest request, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    public abstract void MapEndpoint(WebApplication app);
}

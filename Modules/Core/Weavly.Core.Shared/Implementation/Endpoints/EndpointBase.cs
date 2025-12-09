using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Implementation.Endpoints;

public abstract class EndpointBase<TRequest>(IMessageBus bus) : IWeavlyEndpoint<TRequest>
    where TRequest : IWeavlyCommand
{
    private bool authorize;

    private string[] policies = [];

    public virtual async Task<IResult> HandleAsync(TRequest request, CancellationToken ct = default)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    public abstract RouteHandlerBuilder Map(WebApplication app);

    public void MapEndpoint(WebApplication app)
    {
        var builder = Map(app);

        if (authorize)
        {
            builder.RequireAuthorization(policies);
        }
    }

    protected void Authorize(params string[] policies)
    {
        this.authorize = true;
        this.policies = policies;
    }
}

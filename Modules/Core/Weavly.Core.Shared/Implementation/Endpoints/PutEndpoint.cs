using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Implementation.Endpoints;

public abstract class PutEndpoint<TRequest, TResponse, TModule>(string path, IMessageBus bus)
    : EndpointBase<TRequest, TResponse>(bus)
    where TRequest : IWeavlyCommand
    where TResponse : class
    where TModule : IWeavlyModule
{
    protected override RouteHandlerBuilder Map(WebApplication app)
    {
        return app.MapPut(path, HandleAsync).WithTags(typeof(TModule).Name);
    }

    public override Task<IResult> HandleAsync([FromBody] TRequest request, CancellationToken ct = default)
    {
        return base.HandleAsync(request, ct);
    }
}
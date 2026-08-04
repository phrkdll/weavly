using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Implementation.Endpoints;

public abstract class GetEndpoint<TRequest,TResponse, TModule>(string path, IMessageBus bus) : EndpointBase<TRequest, TResponse>(bus)
    where TRequest : IWeavlyCommand
    where TResponse : class
    where TModule : IWeavlyModule
{
    protected override RouteHandlerBuilder Map(WebApplication app)
    {
        return app.MapGet(path, HandleAsync).WithTags(typeof(TModule).Name);
    }

    public override Task<IResult> HandleAsync([AsParameters] TRequest request, CancellationToken ct = default)
    {
        return base.HandleAsync(request, ct);
    }
}
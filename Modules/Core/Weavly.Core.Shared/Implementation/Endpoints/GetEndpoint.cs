using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Implementation.Endpoints;

public abstract class GetEndpoint<TRequest, TModule>(string path, IMessageBus bus) : EndpointBase<TRequest>(bus)
    where TRequest : IWeavlyCommand
    where TModule : IWeavlyModule
{
    public override RouteHandlerBuilder Map(WebApplication app) =>
        app.MapGet(path, HandleAsync).WithTags(typeof(TModule).Name);

    public override Task<IResult> HandleAsync([AsParameters] TRequest request, CancellationToken ct = default) =>
        base.HandleAsync(request, ct);
}

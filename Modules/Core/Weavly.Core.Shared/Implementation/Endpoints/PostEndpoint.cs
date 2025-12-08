using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Implementation.Endpoints;

public abstract class PostEndpoint<TRequest, TModule>(string path, IMessageBus bus) : EndpointBase<TRequest>(bus)
    where TRequest : IWeavlyCommand
    where TModule : IWeavlyModule
{
    public override RouteHandlerBuilder Map(WebApplication app) =>
        app.MapPost(path, HandleAsync).WithTags(typeof(TModule).Name);

    public override Task<IResult> HandleAsync([FromBody] TRequest request, CancellationToken ct) =>
        base.HandleAsync(request, ct);
}

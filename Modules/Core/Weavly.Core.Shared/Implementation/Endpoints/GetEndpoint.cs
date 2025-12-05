using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Implementation.Endpoints;

public abstract class GetEndpoint<TRequest, TModule>(string path, IMessageBus bus)
    : EndpointBase<TRequest, TModule>(bus)
    where TRequest : IWeavlyCommand
    where TModule : IWeavlyModule
{
    public override void MapEndpoint(WebApplication app) =>
        app.MapGet(path, HandleAsync).WithTags(typeof(TModule).Name);
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyEndpoint<TRequest> : IWeavlyEndpoint
    where TRequest : IWeavlyCommand
{
    Task<IResult> HandleAsync(TRequest request, CancellationToken ct);
}

public interface IWeavlyEndpoint
{
    void MapEndpoint(WebApplication app);
}

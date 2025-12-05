using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyEndpoint<TRequest> : IWeavlyEndpoint
    where TRequest : IWeavlyCommand
{
    void MapEndpoint(WebApplication app);

    Task<IResult> HandleAsync(TRequest request, CancellationToken ct);
}

public interface IWeavlyEndpoint;

using Microsoft.AspNetCore.Builder;

namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyEndpoint
{
    void MapEndpoint(WebApplication app);
}

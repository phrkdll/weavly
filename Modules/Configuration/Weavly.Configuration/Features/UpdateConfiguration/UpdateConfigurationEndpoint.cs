using Weavly.Configuration.Shared.Features.UpdateConfiguration;
using Weavly.Core.Shared.Implementation.Endpoints;
using Weavly.Core.Shared.Models;
using Wolverine;

namespace Weavly.Configuration.Features.UpdateConfiguration;

public sealed class UpdateConfigurationEndpoint
    : PutEndpoint<UpdateConfigurationCommand, EmptyResponse, ConfigurationModule>
{
    public UpdateConfigurationEndpoint(IMessageBus bus)
        : base("configuration", bus)
    {
        Authorize();
    }
}

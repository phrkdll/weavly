using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Configuration.Features.LoadConfiguration;

public class
    LoadConfigurationEndpoint : GetEndpoint<LoadConfigurationCommand, LoadConfigurationResponse, ConfigurationModule>
{
    public LoadConfigurationEndpoint(IMessageBus bus) : base("configuration", bus)
    {
        Authorize();
    }
}
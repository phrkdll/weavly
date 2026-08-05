using Weavly.Configuration.Shared.Features.ListConfigurationModules;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Configuration.Features.ListConfigurationModules;

public sealed class ListConfigurationModulesEndpoint
    : GetEndpoint<ListConfigurationModulesCommand, ListConfigurationModulesResponse, ConfigurationModule>
{
    public ListConfigurationModulesEndpoint(IMessageBus bus)
        : base("configuration/modules", bus)
    {
        Authorize();
    }
}

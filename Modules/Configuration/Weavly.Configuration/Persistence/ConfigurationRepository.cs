using Weavly.Configuration.Models;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Shared.Persistence;

namespace Weavly.Configuration.Persistence;

public sealed class ConfigurationRepository(IWeavlyRepository<ConfigurationModule> repository)
    : ModuleRepository<ConfigurationModule>(repository)
{
    public IWeavlyCollection<AppConfiguration, ConfigurationId> Configurations =>
        this.Repository.GetCollectionFor<AppConfiguration, ConfigurationId>();
}
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Shared.Features.LoadConfiguration;

public sealed record LoadConfigurationCommand(string? Module) : IWeavlyCommand
{
    public static LoadConfigurationCommand Create<TModule>()
    {
        return Create(typeof(TModule).Name);
    }

    public static LoadConfigurationCommand Create(string module)
    {
        return new LoadConfigurationCommand(module);
    }
}
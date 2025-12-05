using System.Text.Json.Serialization;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Shared.Features.LoadConfiguration;

public sealed class LoadConfigurationCommand : IWeavlyCommand
{
    public string Module { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    [JsonConstructor]
    private LoadConfigurationCommand() { }

    public static LoadConfigurationCommand Create<TModule>(string category = "Default")
    {
        return Create(typeof(TModule).Name, category);
    }

    public static LoadConfigurationCommand Create(string module, string category = "Default")
    {
        return new LoadConfigurationCommand { Module = module, Category = category };
    }
}

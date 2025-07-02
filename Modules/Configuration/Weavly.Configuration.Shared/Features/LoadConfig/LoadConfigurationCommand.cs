using System.Text.Json.Serialization;
using FastEndpoints;

namespace Weavly.Configuration.Shared.Features.LoadConfig;

public sealed class LoadConfigurationCommand : ICommand<Result>
{
    public string Module { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;

    [JsonConstructor]
    private LoadConfigurationCommand() { }

    public static LoadConfigurationCommand Create<TModule>(string category = "Default")
    {
        return Create(typeof(TModule).Name, category);
    }

    private static LoadConfigurationCommand Create(string module, string category = "Default")
    {
        return new LoadConfigurationCommand { Module = module, Category = category };
    }
}

using System.Text.Json.Serialization;

namespace Weavly.Configuration.Shared.Features.LoadConfiguration;

public sealed record LoadConfigurationResponse(string Module, IEnumerable<ConfigurationResponse> Items)
{
    [JsonIgnore]
    public ConfigurationResponse this[string i] => Items.Single(x => x.Name == i);
}

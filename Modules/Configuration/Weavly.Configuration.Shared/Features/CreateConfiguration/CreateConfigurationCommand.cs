using System.Text.Json.Serialization;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Shared.Features.CreateConfiguration;

public sealed record CreateConfigurationCommand : ConfigurationResponse, IWeavlyCommand
{
    [JsonConstructor]
    private CreateConfigurationCommand() { }

    public static CreateConfigurationCommand Create<TModule>(string name, object value, string category = "Default")
    {
        var command = new CreateConfigurationCommand
        {
            Module = typeof(TModule).Name,
            Name = name,
            Category = category,
        };

        return value switch
        {
            string s => command with { StringValue = s },
            int i => command with { IntValue = i },
            bool b => command with { BoolValue = b },
            double d => command with { DoubleValue = d },
            _ => throw new InvalidOperationException("Unsupported configuration value type"),
        };
    }
}

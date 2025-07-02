using System.Text.Json.Serialization;
using FastEndpoints;

namespace Weavly.Configuration.Shared;

public sealed record CreateConfigurationCommand : ConfigurationResponse, ICommand<Result>
{
    [JsonConstructor]
    private CreateConfigurationCommand() { }

    public static CreateConfigurationCommand Create<TModule>(string name, object value, string category = "Default")
    {
        var item = new CreateConfigurationCommand
        {
            Module = typeof(TModule).Name,
            Name = name,
            Category = category,
        };

        return value switch
        {
            string s => item with { StringValue = s },
            int i => item with { IntValue = i },
            bool b => item with { BoolValue = b },
            double d => item with { DoubleValue = d },
            _ => throw new InvalidOperationException("Unsupported configuration value type"),
        };
    }
}

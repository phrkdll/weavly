using Weavly.Configuration.Shared.Models;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Shared.Features.CreateConfiguration;

public sealed record CreateConfigurationCommand(string Module, string Name, string Category) : IWeavlyCommand
{
    public string? StringValue { get; init; }
    public bool? BoolValue { get; set; }
    public int? IntValue { get; init; }
    public double? DoubleValue { get; init; }

    public ConfigurationValueType ValueType { get; init; }

    public static CreateConfigurationCommand Create<TModule>(string name, object value, string category)
    {
        var command = new CreateConfigurationCommand(typeof(TModule).Name, name, category);

        return value switch
        {
            string s => command with { StringValue = s, ValueType = ConfigurationValueType.String },
            int i => command with { IntValue = i, ValueType = ConfigurationValueType.Int },
            bool b => command with { BoolValue = b, ValueType = ConfigurationValueType.Bool },
            double d => command with { DoubleValue = d, ValueType = ConfigurationValueType.Double },
            _ => throw new InvalidOperationException("Unsupported configuration value type"),
        };
    }
}
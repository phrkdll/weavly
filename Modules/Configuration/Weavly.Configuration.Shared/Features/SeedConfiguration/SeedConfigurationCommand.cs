using Weavly.Configuration.Shared.Models;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Shared.Features.SeedConfiguration;

public sealed record SeedConfigurationCommand(string Module, string Name, string Category) : IWeavlyCommand
{
    public string? StringValue { get; init; }
    public bool? BoolValue { get; init; }
    public int? IntValue { get; init; }
    public double? DoubleValue { get; init; }
    public ConfigurationValueType ValueType { get; init; }

    public static SeedConfigurationCommand Create<TModule>(string name, object value, string category)
    {
        var command = new SeedConfigurationCommand(typeof(TModule).Name, name, category);

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

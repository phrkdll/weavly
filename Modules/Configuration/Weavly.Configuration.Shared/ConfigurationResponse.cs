using Weavly.Configuration.Shared.Identifiers;

namespace Weavly.Configuration.Shared;

public record ConfigurationResponse
{
    public ConfigurationId? Id { get; init; }

    public string Module { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? StringValue { get; init; }

    public int? IntValue { get; init; }

    public double? DoubleValue { get; init; }

    public bool? BoolValue { get; init; }

    public string AsString()
    {
        return StringValue ?? string.Empty;
    }

    public int AsInt()
    {
        return IntValue ?? 0;
    }

    public double AsDouble()
    {
        return DoubleValue ?? 0;
    }

    public bool AsBool()
    {
        return BoolValue ?? false;
    }

    public static ConfigurationResponse Create(string name)
    {
        return new ConfigurationResponse { Name = name };
    }
}
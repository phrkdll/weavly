using System.Diagnostics.CodeAnalysis;

namespace Weavly.Configuration.Shared.Events;

[ExcludeFromCodeCoverage]
public sealed record ConfigurationChangedEvent(string Module, string Name, string Category = "Default")
{
    public string? StringValue { get; set; }

    public int? IntValue { get; set; }

    public bool? BoolValue { get; set; }
}

namespace Weavly.Configuration.Shared.Events;

public sealed record ConfigurationChangedEvent(string Module, string Name, string Category = "Default")
{
    public string? StringValue { get; set; }

    public int? IntValue { get; set; }

    public bool? BoolValue { get; set; }
}

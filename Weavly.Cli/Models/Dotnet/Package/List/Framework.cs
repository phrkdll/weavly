using System.Text.Json.Serialization;

namespace Weavly.Cli.Models.Dotnet.Package.List;

[Serializable]
public sealed class Framework
{
    [JsonPropertyName("framework")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("topLevelPackages")]
    public IEnumerable<ListPackage> Packages { get; init; } = [];
}

public sealed class ListPackage
{
    public string Id { get; init; } = string.Empty;

    public string RequestedVersion { get; init; } = string.Empty;

    public string ResolvedVersion { get; init; } = string.Empty;
}

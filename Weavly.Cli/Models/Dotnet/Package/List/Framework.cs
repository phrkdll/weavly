using System.Text.Json.Serialization;

namespace Weavly.Cli.Models.Dotnet.Package.List;

public sealed class Framework
{
    [JsonPropertyName("framework")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("topLevelPackages")]
    public IList<ListPackage> Packages { get; set; } = [];
}

public sealed class ListPackage
{
    public string Id { get; set; } = string.Empty;

    public string RequestedVersion { get; set; } = string.Empty;

    public string ResolvedVersion { get; set; } = string.Empty;
}

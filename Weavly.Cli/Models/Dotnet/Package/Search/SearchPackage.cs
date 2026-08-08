namespace Weavly.Cli.Models.Dotnet.Package.Search;

[Serializable]
public class SearchPackage
{
    public string Id { get; init; } = string.Empty;

    public string LatestVersion { get; init; } = string.Empty;
}

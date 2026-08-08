namespace Weavly.Cli.Models.Dotnet.Package.Search;

[Serializable]
public class SearchResult
{
    public string SourceName { get; init; } = string.Empty;

    public IEnumerable<SearchPackage> Packages { get; init; } = [];
}

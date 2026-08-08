namespace Weavly.Cli.Models.Dotnet.Package.Search;

[Serializable]
public sealed class Search
{
    public int Version { get; init; }

    public IEnumerable<SearchResult> SearchResult { get; init; } = [];
}

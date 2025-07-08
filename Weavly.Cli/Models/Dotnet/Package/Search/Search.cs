namespace Weavly.Cli.Models.Dotnet.Package.Search;

public sealed class Search
{
    public int Version { get; set; }

    public IList<SearchResult> SearchResult { get; set; } = [];
}

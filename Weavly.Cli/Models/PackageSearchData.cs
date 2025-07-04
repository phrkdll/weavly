namespace Weavly.Cli.Models;

public sealed class PackageSearchData
{
    public int Version { get; set; }

    public IList<SearchResult> SearchResult { get; set; } = [];
}

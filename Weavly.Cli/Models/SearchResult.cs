namespace Weavly.Cli.Models;

public class SearchResult
{
    public string SourceName { get; set; } = string.Empty;

    public IList<Package> Packages { get; set; } = [];
}

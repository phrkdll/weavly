namespace Weavly.Cli.Models.Dotnet.Package.Search;

public class SearchResult
{
    public string SourceName { get; set; } = string.Empty;

    public IEnumerable<SearchPackage> Packages { get; } = [];
}
namespace Weavly.Cli.Models.Dotnet.Package.Search;

public class SearchResult
{
    public string SourceName { get; set; } = string.Empty;

    public IList<SearchPackage> Packages { get; set; } = [];
}

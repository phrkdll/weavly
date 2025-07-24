namespace Weavly.Cli.Models.Dotnet.Package.Search;

public class SearchPackage
{
    public string Id { get; set; } = string.Empty;

    public string LatestVersion { get; set; } = string.Empty;

    public int TotalDownloads { get; set; }

    public string Owners { get; set; } = string.Empty;
}

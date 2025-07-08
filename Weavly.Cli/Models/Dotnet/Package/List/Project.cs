namespace Weavly.Cli.Models.Dotnet.Package.List;

public sealed class Project
{
    public string Path { get; set; } = string.Empty;

    public IList<Framework> Frameworks { get; set; } = [];
}

namespace Weavly.Cli.Models.Dotnet.Package.List;

public sealed class List
{
    public int Version { get; set; }

    public IList<Project> Projects { get; set; } = [];
}

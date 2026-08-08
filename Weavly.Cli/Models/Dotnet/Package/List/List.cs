namespace Weavly.Cli.Models.Dotnet.Package.List;

[Serializable]
public sealed class List
{
    public int Version { get; init; }

    public IEnumerable<Project> Projects { get; init; } = [];
}

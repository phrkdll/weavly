namespace Weavly.Cli.Models.Dotnet.Package.List;

[Serializable]
public sealed class Project
{
    public string Path { get; init; } = string.Empty;

    public IEnumerable<Framework> Frameworks { get; init; } = [];
}

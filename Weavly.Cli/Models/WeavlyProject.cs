namespace Weavly.Cli.Models;

public sealed class WeavlyProject
{
    private WeavlyProject(string name, string fullName, string? suffix)
    {
        Name = name;
        FullName = suffix is null ? fullName : $"{fullName}.{suffix}";

        Folder = Path.Combine("Modules", Name, FullName);
        File = Path.Combine(Folder, $"{FullName}.csproj");
    }

    public string Name { get; }

    public string FullName { get; }

    public string Folder { get; }

    public string File { get; }

    public static WeavlyProject New(string name, string fullName, string? suffix = null)
    {
        return new WeavlyProject(name, fullName, suffix);
    }

    public override string ToString()
    {
        return Folder;
    }
}
namespace Weavly.Cli.Models;

public sealed class WeavlyProject
{
    public string Name { get; }

    public string FullName { get; }

    public string Folder { get; }

    public string File { get; }

    public WeavlyProject(string name, string fullName, string? suffix)
    {
        this.Name = name;
        this.FullName = suffix is null ? fullName : $"{fullName}.{suffix}";

        this.Folder = Path.Combine("Modules", this.Name, this.FullName);
        this.File = Path.Combine(this.Folder, $"{this.FullName}.csproj");
    }

    public static WeavlyProject New(string name, string fullName, string? suffix = null) => new(name, fullName, suffix);

    public override string ToString() => Folder;
}

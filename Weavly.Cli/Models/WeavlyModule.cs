namespace Weavly.Cli.Models;

public sealed class WeavlyModule
{
    public string Solution { get; }

    public string Name { get; }

    public string FullName { get; }

    public WeavlyProject Main => WeavlyProject.New(Name, FullName);

    public WeavlyProject Shared => WeavlyProject.New(Name, FullName, nameof(Shared));

    public WeavlyProject Tests => WeavlyProject.New(Name, FullName, nameof(Tests));

    public WeavlyModule(string name, string solution)
    {
        this.Name = name;
        this.Solution = solution;

        this.FullName = $"{Solution}.{Name}";
    }

    public static WeavlyModule New(string name, string solution) => new(name, solution);
}

namespace Weavly.Cli.Models;

public sealed class WeavlyModule
{
    private WeavlyModule(string name, string solution)
    {
        Name = name;
        Solution = solution;

        FullName = $"{Solution}.{Name}";
    }

    public string Solution { get; }

    public string Name { get; }

    public string FullName { get; }

    public WeavlyProject Main => WeavlyProject.New(Name, FullName);

    public WeavlyProject Shared => WeavlyProject.New(Name, FullName, nameof(Shared));

    public WeavlyProject Tests => WeavlyProject.New(Name, FullName, nameof(Tests));

    public static WeavlyModule New(string name, string solution) => new(name, solution);
}

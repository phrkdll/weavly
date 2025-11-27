using System.Text;

namespace Weavly.Cli.Models.ProcessRunner;

public class Dotnet : ProcessRunnerCommand
{
    private Dotnet(string arguments)
        : base("dotnet", arguments) { }

    public static Dotnet AddPackage(WeavlyProject to, string package) => new(BuildArguments("package", to, package));

    public static Dotnet AddReference(WeavlyProject to, params string[] from) =>
        new(BuildArguments("reference", to, from));

    public static Dotnet AddProject(string template, WeavlyProject output) =>
        new($"new {template} -o {output} --no-restore");

    public static Dotnet AddToSolution(params WeavlyProject[] projects)
    {
        var args = new StringBuilder();
        args.Append("sln add ");

        foreach (var project in projects)
        {
            args.Append($"{project} ");
        }

        return new Dotnet(args.ToString());
    }

    public static Dotnet NewSolution(string output, string format) => new($"new sln -o {output} -f {format}");

    public static Dotnet Custom(string arguments) => new(arguments);

    private static string BuildArguments(string type, WeavlyProject to, params string[] from)
    {
        var sb = new StringBuilder();

        sb.Append(type);
        sb.Append(" add ");

        foreach (var project in from)
        {
            sb.Append($"{project} ");
        }
        sb.Append($"--project {to}");

        return sb.ToString();
    }
}

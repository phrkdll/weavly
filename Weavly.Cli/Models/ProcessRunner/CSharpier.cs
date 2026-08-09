namespace Weavly.Cli.Models.ProcessRunner;

public sealed class CSharpier : ProcessRunnerCommand
{
    private CSharpier(string arguments)
        : base("csharpier", arguments) { }

    public static CSharpier Check() => new("check .");

    public static CSharpier Format() => new("format .");
}

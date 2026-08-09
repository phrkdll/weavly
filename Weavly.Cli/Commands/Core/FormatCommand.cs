using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models.ProcessRunner;

namespace Weavly.Cli.Commands.Core;

[Description("Formats files within the solution (utilizes CSharpier)")]
public sealed class FormatCommand : InterruptibleAsyncCommand<FormatCommand.Settings>
{
    protected override async Task HandleAsync(
        CommandContext commandContext,
        Settings settings,
        CancellationToken ct = default
    )
    {
        string result;
        if (settings.CheckOnly)
        {
            result = await Runner.RunAsync(CSharpier.Check(), ct);
        }
        else
        {
            result = await Runner.RunAsync(CSharpier.Format(), ct);
        }

        AnsiConsole.WriteLine(result);
    }

    [Serializable]
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-c|--check-only")]
        [Description("Only check for formatting issues")]
        public bool CheckOnly { get; set; }
    }
}

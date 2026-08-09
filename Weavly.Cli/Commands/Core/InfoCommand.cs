using System.ComponentModel;
using Spectre.Console.Cli;

namespace Weavly.Cli.Commands.Core;

[Description("Displays information about the current solution")]
public sealed class InfoCommand : InterruptibleAsyncCommand<InfoCommand.Settings>
{
    protected override Task HandleAsync(
        CommandContext commandContext,
        Settings settings,
        CancellationToken ct = default
    )
    {
        throw new NotImplementedException();
    }

    [Serializable]
    public sealed class Settings : CommandSettings { }
}

using System.ComponentModel;
using Spectre.Console.Cli;

namespace Weavly.Cli.Commands.Core;

[Description("Update Weavly packages within the solution")]
public sealed class UpdateCommand : InterruptibleAsyncCommand<UpdateCommand.Settings>
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
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-v|--version <version>")]
        [Description("The version of Weavly to be used.")]
        [DefaultValue("latest")]
        public string? Version { get; init; }
    }
}

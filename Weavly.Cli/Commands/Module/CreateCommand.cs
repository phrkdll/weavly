using System.ComponentModel;
using Spectre.Console.Cli;

namespace Weavly.Cli.Commands.Module;

[Description("Initializes a new custom module project for the current solution")]
public class CreateCommand : InterruptibleAsyncCommand<CreateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-n|--name <name>")]
        [Description("Module name")]
        public string? ModuleName { get; set; }
    }

    public override Task HandleAsync(CommandContext commandContext, Settings settings)
    {
        throw new NotImplementedException();
    }
}

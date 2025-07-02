using System.ComponentModel;
using Spectre.Console.Cli;

namespace Weavly.Cli.Commands.Module;

[Description("Adds an existing Weavly module to the solution")]
public class AddCommand : InterruptibleAsyncCommand<AddCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-n|--name <name>")]
        [Description("Module name")]
        public string? ModuleName { get; set; }
    }

    public override Task HandleAsync(CommandContext _, Settings settings)
    {
        throw new NotImplementedException();
    }
}

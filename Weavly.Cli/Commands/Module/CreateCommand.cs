using System.ComponentModel;
using Spectre.Console;
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

        [CommandOption("-t|--testing-framework <name>")]
        [Description("Testing framework (as listed in 'dotnet new list')")]
        public string TestingFramework { get; set; } = "xunit";
    }

    public override async Task HandleAsync(CommandContext commandContext, Settings settings, CancellationToken ct)
    {
        var solutionName =
            Directory.GetFiles(".", "*.sln*").Select(Path.GetFileNameWithoutExtension).FirstOrDefault()
            ?? throw new InvalidOperationException("No solution file found in the current directory.");

        var moduleName =
            settings.ModuleName
            ?? await new TextPrompt<string>("Please enter a module name:").ShowAsync(AnsiConsole.Console, ct);

        var fullModuleName = $"{solutionName}.{moduleName}";
        var modulePath = Path.Combine("Modules", moduleName, fullModuleName);

        await Runner
            .WithMessage($"Creating module [teal]{moduleName}[/]...\n")
            .RunAsync("dotnet", $"new classlib -o {modulePath}.Shared", ct);

        await Runner.RunAsync("dotnet", $"new classlib -o {modulePath}", ct);

        await Runner.RunAsync("dotnet", $"new {settings.TestingFramework} -o {modulePath}.Tests", ct);

        // TODO: Add project references and solution additions
        // + Remove unnecessary entries from .csproj files
    }
}

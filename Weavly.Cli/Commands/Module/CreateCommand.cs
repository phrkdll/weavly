using System.ComponentModel;
using System.Xml.Linq;
using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models;

namespace Weavly.Cli.Commands.Module;

[Description("Initializes a new custom module project for the current solution")]
public class CreateCommand : InterruptibleAsyncCommand<CreateCommand.Settings>
{
    private readonly WeavlyModule coreModule = WeavlyModule.New("Core", "Weavly");

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

        var module = WeavlyModule.New(moduleName, solutionName);

        AnsiConsole.Write(new Markup($"Creating module [teal]{module.FullName}[/]...\n"));
        await Runner
            .WithMessage($"Adding project [teal]{module.Main}[/]...\n")
            .RunDotnetAsync($"new classlib -o {module.Main}", ct);

        await Runner
            .WithMessage($"Adding project [teal]{module.Shared}[/]...\n")
            .RunDotnetAsync($"new classlib -o {module.Shared}", ct);

        await Runner
            .WithMessage($"Adding project [teal]{module.Tests}[/]...\n")
            .RunDotnetAsync($"new {settings.TestingFramework} -o {module.Tests}", ct);

        await Runner
            .WithMessage($"Adding project/package references...\n")
            .RunDotnetAsync($"reference add {module.Shared} --project {module.Main}", ct);
        await Runner.RunDotnetAsync($"reference add {module.Main} {module.Shared} --project {module.Tests}", ct);

        if (solutionName == coreModule.Solution)
        {
            await Runner.RunDotnetAsync($"reference add {coreModule.Main} --project {module.Main}", ct);
            await Runner.RunDotnetAsync($"reference add {coreModule.Shared} --project {module.Shared}", ct);
        }
        else
        {
            await Runner.RunDotnetAsync($"package add {coreModule.Main.FullName} --project {module.Main}", ct);
            await Runner.RunDotnetAsync($"package add {coreModule.Shared.FullName} --project {module.Shared}", ct);
        }

        await Runner
            .WithMessage($"Adding projects to solution...\n")
            .RunDotnetAsync($"sln add {module.Main} {module.Shared} {module.Tests}", ct);

        // TODO: Clean up project files (e.g., remove default Class1.cs, adjust namespaces, etc.)
        // + Remove unnecessary entries from .csproj files
    }
}

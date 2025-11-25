using System.ComponentModel;
using System.Text;
using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models;
using Weavly.Cli.Models.ProcessRunner;

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
        [DefaultValue("xunit")]
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
            .RunAsync(Dotnet.AddProject("classlib", module.Main), ct);

        await Runner
            .WithMessage($"Adding project [teal]{module.Shared}[/]...\n")
            .RunAsync(Dotnet.AddProject("classlib", module.Shared), ct);

        await Runner
            .WithMessage($"Adding project [teal]{module.Tests}[/]...\n")
            .RunAsync(Dotnet.AddProject("classlib", module.Tests), ct);

        switch (settings.TestingFramework.ToLowerInvariant().Trim())
        {
            case "xunit":
                await Runner.RunAsync(
                    Dotnet.AddPackage(
                        module.Tests,
                        "coverlet.collector",
                        "Microsoft.NET.Test.Sdk",
                        "xunit",
                        "xunit.runner.visualstudio"
                    ),
                    ct
                );
                break;
            case "nunit":
                await Runner.RunAsync(
                    Dotnet.AddPackage(
                        module.Tests,
                        "coverlet.collector",
                        "Microsoft.NET.Test.Sdk",
                        "NUnit",
                        "NUnit.Analyzers",
                        "NUnit3TestAdapter"
                    ),
                    ct
                );
                break;
            default:
                throw new InvalidOperationException($"Unsupported testing framework: {settings.TestingFramework}");
        }

        await Runner
            .WithMessage($"Adding project/package references...\n")
            .RunAsync(Dotnet.AddReference(module.Main, module.Shared.Folder), ct);
        await Runner.RunAsync(Dotnet.AddReference(module.Tests, module.Main.Folder, module.Shared.Folder), ct);

        if (solutionName == coreModule.Solution)
        {
            await Runner.RunAsync(Dotnet.AddReference(module.Main, coreModule.Main.Folder), ct);
            await Runner.RunAsync(Dotnet.AddReference(module.Shared, coreModule.Shared.Folder), ct);
        }
        else
        {
            await Runner.RunAsync(Dotnet.AddPackage(module.Main, coreModule.Main.FullName), ct);
            await Runner.RunAsync(Dotnet.AddPackage(module.Shared, coreModule.Shared.FullName), ct);
        }

        await Runner
            .WithMessage($"Adding projects to solution...\n")
            .RunAsync(Dotnet.AddToSolution(module.Main, module.Shared, module.Tests), ct);

        // TODO: Clean up project files (e.g., remove default Class1.cs, adjust namespaces, etc.)
        // + Remove unnecessary entries from .csproj files
    }
}

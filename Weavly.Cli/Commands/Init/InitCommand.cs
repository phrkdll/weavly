using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models;

namespace Weavly.Cli.Commands;

[Description("Initialize a new Weavly solution")]
public class InitCommand : InterruptibleAsyncCommand<InitCommand.Settings>
{
    private const string DefaultSolutionName = ".";

    private const string DotnetCommand = "dotnet";

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[name]")]
        [Description("Solution name")]
        public string? SolutionName { get; set; }

        [CommandOption("-x|--slnx")]
        [Description("Use new solution (.slnx) format")]
        public bool NewSolutionFormat { get; set; }

        [CommandOption("-p|--project <name>")]
        [Description("Project name (for the initial project)")]
        public string? ProjectName { get; set; }
    }

    public override async Task HandleAsync(CommandContext _, Settings settings)
    {
        var solutionNameInput =
            settings.SolutionName
            ?? await new TextPrompt<string>("Please enter a solution name:")
                .DefaultValue(DefaultSolutionName)
                .ShowAsync(AnsiConsole.Console, Token);

        var solutionName =
            solutionNameInput == DefaultSolutionName
                ? Directory.GetCurrentDirectory().Split(DirectorySeparator).Last()
                : solutionNameInput;

        var projectName =
            settings.ProjectName
            ?? await new TextPrompt<string>("Please enter a project name:")
                .DefaultValue(solutionName + ".Api")
                .ShowAsync(AnsiConsole.Console, Token);

        var workingDir =
            solutionNameInput == DefaultSolutionName
                ? Directory.GetCurrentDirectory()
                : Path.Combine(Directory.GetCurrentDirectory(), solutionName);

        var solutionFormat =
            settings.NewSolutionFormat
            || await AnsiConsole.PromptAsync(
                new TextPrompt<bool>("Use new XML solution (.slnx) format?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(true)
                    .WithConverter(choice => choice ? "y" : "n")
            )
                ? "slnx"
                : "sln";

        await Runner
            .WithMessage($"Initializing solution [teal]'{solutionName}'[/]...\n")
            .RunAsync(DotnetCommand, $"new sln -o {solutionNameInput} -f {solutionFormat}");

        var result = await Runner
            .InDirectory(workingDir)
            .ParseJsonAsync<PackageSearchData>(
                DotnetCommand,
                $"package search weavly --format json --verbosity detailed"
            );

        var choices =
            result
                ?.SearchResult.First()
                .Packages.Where(x => !x.Id.EndsWith(".Cli") && !x.Id.EndsWith(".Shared") && !x.Id.Contains(".Core"))
                .Select(x => x.Id) ?? [];

        var selectedProviders = await new MultiSelectionPrompt<string>()
            .Title("Which [teal]modules[/] do you want to use?")
            .Required()
            .PageSize(5)
            .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
            .AddChoices(choices)
            .ShowAsync(AnsiConsole.Console, Token);

        await Runner
            .InDirectory(workingDir)
            .WithMessage($"Adding project [teal]'{projectName}'[/]...\n")
            .RunAsync(DotnetCommand, $"new web -n {projectName}");
        await Runner.InDirectory(workingDir).RunAsync(DotnetCommand, $"sln add {projectName}");

        foreach (var module in selectedProviders)
        {
            await Runner
                .InDirectory(workingDir)
                .WithMessage($"Adding module [teal]{module}[/]...\n")
                .RunAsync(DotnetCommand, $"package add {module} --project ./{projectName}/{projectName}.csproj");
        }
    }
}

using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Weavly.Cli.Commands;

[Description("Initialize a new Weavly solution")]
public class InitCommand : InterruptibleAsyncCommand<InitCommand.Settings>
{
    private const string DefaultSolutionName = ".";

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

        await Runner
            .WithMessage($"Initializing solution [teal]'{solutionName}'[/]...\n")
            .RunAsync("dotnet", $"new sln -o {solutionNameInput}");

        var workingDir =
            solutionNameInput == DefaultSolutionName
                ? Directory.GetCurrentDirectory()
                : Path.Combine(Directory.GetCurrentDirectory(), solutionName);

        if (settings.NewSolutionFormat)
        {
            await Runner.InDirectory(workingDir).RunAsync("dotnet", $"sln migrate");
            await Runner.InDirectory(workingDir).RunAsync("rm", $"{solutionName}.sln");
        }

        await Runner.InDirectory(workingDir).RunAsync("dotnet", $"new web -n {projectName}");
        await Runner.InDirectory(workingDir).RunAsync("dotnet", $"sln add {projectName}");
    }
}

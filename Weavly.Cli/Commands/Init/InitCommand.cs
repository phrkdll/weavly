using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models;

namespace Weavly.Cli.Commands.Init;

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

    public override async Task HandleAsync(CommandContext commandContext, Settings settings)
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

        List<string> selectedModules =
        [
            "Weavly.Core",
            .. await new MultiSelectionPrompt<string>()
                .Title("Which [teal]modules[/] do you want to use?")
                .Required()
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
                .AddChoices(await GetAvailableWeavlyPackages(workingDir))
                .ShowAsync(AnsiConsole.Console, Token),
        ];

        await Runner
            .InDirectory(workingDir)
            .WithMessage($"Adding project [teal]'{projectName}'[/]...\n")
            .RunAsync(DotnetCommand, $"new web -n {projectName}");
        await Runner.InDirectory(workingDir).RunAsync(DotnetCommand, $"sln add {projectName}");

        foreach (var module in selectedModules)
        {
            await Runner
                .InDirectory(workingDir)
                .WithMessage($"Adding module [teal]{module}[/]...\n")
                .RunAsync(DotnetCommand, $"package add {module} --project ./{projectName}/{projectName}.csproj");
        }

        await InitProgramBootstrapper(workingDir, projectName, selectedModules);
    }

    private async Task InitProgramBootstrapper(string workingDir, string projectName, List<string> selectedModules)
    {
        var programFilePath = Path.Combine(workingDir, projectName, "Program.cs");

        var file = await File.ReadAllTextAsync(programFilePath, Token);

        if (file == null)
        {
            return;
        }

        AddUsings(selectedModules, ref file);
        AddBuilderSetup(selectedModules, ref file);
        AddAppSetup(ref file);
        RemoveEndpointMappings(ref file);

        await File.WriteAllTextAsync(programFilePath, file, Token);
    }

    private static void RemoveEndpointMappings(ref string file)
    {
        string pattern = @"(.*app.Map.*;\s*app)";
        string replacement = "app";

        file = Regex.Replace(file, pattern, replacement);
    }

    private static void AddAppSetup(ref string file)
    {
        string pattern = @"(.*app.Run.*;)";
        string replacement = $"app.UseWeavly();\n\n$1";

        file = Regex.Replace(file, pattern, replacement);
    }

    private static void AddBuilderSetup(List<string> selectedModules, ref string file)
    {
        var setupPattern = @"(var builder.*;)";
        var setupBuilder = new StringBuilder("$1\n\nbuilder.AddWeavly()\n");

        foreach (var module in selectedModules)
        {
            var moduleName = module.Replace("Weavly.", string.Empty).Replace(".", string.Empty);
            setupBuilder.AppendLine($"    .AddModule<{moduleName}Module>()");
        }
        setupBuilder.AppendLine("    .Build();");

        file = Regex.Replace(file, setupPattern, setupBuilder.ToString());
    }

    private static void AddUsings(List<string> selectedModules, ref string file)
    {
        var usingBuilder = new StringBuilder();

        foreach (var module in selectedModules)
        {
            usingBuilder.AppendLine($"using {module};");
        }
        usingBuilder.AppendLine();

        file = $"{usingBuilder}{file}";
    }
}

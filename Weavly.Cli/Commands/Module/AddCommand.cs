using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Weavly.Cli.Commands.Module;

[Description("Adds an existing Weavly module to the solution")]
public class AddCommand : InterruptibleAsyncCommand<AddCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-p|--project <name>")]
        [Description("Project name")]
        public string? ProjectName { get; set; }
    }

    public override async Task HandleAsync(CommandContext commandContext, Settings settings)
    {
        var projects = GetRelevantProjects().ToDictionary(ExtractFileNameWithoutExtension, f => f);
        var projectName =
            settings.ProjectName
            ?? await new SelectionPrompt<string>()
                .Title("Where do you want to add the [teal]module[/]?")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
                .AddChoices(projects.Keys)
                .ShowAsync(AnsiConsole.Console, Token);

        await Runner.WithMessage($"Adding module to [teal]'{projectName}'[/]...\n").RunAsync("echo", "");

        var workingDir = Path.Combine(Directory.GetCurrentDirectory(), projectName);

        List<string> selectedModules =
        [
            .. await new MultiSelectionPrompt<string>()
                .Title("Which [teal]modules[/] do you want to use?")
                .Required()
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
                .AddChoices(await GetAvailableWeavlyPackages(workingDir))
                .ShowAsync(AnsiConsole.Console, Token),
        ];

        Console.WriteLine("Selected modules:");
        foreach (var module in selectedModules)
        {
            Console.WriteLine(module);
        }
    }

    private static string ExtractFileNameWithoutExtension(string fileName)
    {
        var fileNameWithExtension = fileName.Split(DirectorySeparator).Last();

        return fileNameWithExtension[..fileNameWithExtension.LastIndexOf('.')];
    }

    private static string[] GetRelevantProjects()
    {
        return [.. Directory.GetFiles(".", "*.csproj", SearchOption.AllDirectories)];
    }
}

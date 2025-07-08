using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Utils;

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

        AnsiConsole.Write(new Markup($"Adding module to [teal]{projectName}[/]...\n"));

        var workingDir = Path.Combine(Directory.GetCurrentDirectory(), projectName);

        var installedPackages = await ListWeavlyPackagesAsync(projectName);

        List<string> selectedModules =
        [
            .. await new MultiSelectionPrompt<string>()
                .Title("Which [teal]modules[/] do you want to add?")
                .Required()
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
                .AddChoices(await SearchWeavlyPackagesAsync(workingDir, installedPackages))
                .ShowAsync(AnsiConsole.Console, Token),
        ];

        foreach (var module in selectedModules)
        {
            await Runner
                .WithMessage($"Adding module [teal]{module}[/]...\n")
                .RunAsync("dotnet", $"package add {module} --project ./{projectName}/{projectName}.csproj");
        }

        await UpdateProgramBootstrapperAsync(projectName, selectedModules);
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

    private async Task UpdateProgramBootstrapperAsync(string projectName, List<string> selectedModules)
    {
        AnsiConsole.Write(new Markup($"Updating [teal]{projectName}/Program.cs[/]...\n"));

        var programFilePath = Path.Combine(projectName, "Program.cs");

        var file = await File.ReadAllTextAsync(programFilePath, Token);

        if (file == null)
        {
            return;
        }

        ModuleManagementHelper.UpdateUsings(selectedModules, ref file);
        ModuleManagementHelper.UpdateBuilderSetup(selectedModules, ref file);

        await File.WriteAllTextAsync(programFilePath, file, Token);
    }
}

using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models.ProcessRunner;
using Weavly.Cli.Utils;

namespace Weavly.Cli.Commands.Module;

[Description("Adds an existing Weavly module to the solution")]
public sealed class AddModuleCommand : InterruptibleAsyncCommand<AddModuleCommand.Settings>
{
    protected override async Task HandleAsync(
        CommandContext commandContext,
        Settings settings,
        CancellationToken ct = default
    )
    {
        var projects = GetRelevantProjects().ToDictionary(ExtractFileNameWithoutExtension, f => f);
        var projectName = settings.ProjectName ?? await SelectProjectAsync(projects, ct);

        AnsiConsole.Write(new Markup($"Adding module to [teal]{projectName}[/]...\n"));

        var workingDir = Path.Combine(Directory.GetCurrentDirectory(), projectName);

        var installedPackages = await ListWeavlyPackagesAsync(projectName, ct: ct);

        List<string> selectedModules =
        [
            .. await new MultiSelectionPrompt<string>()
                .Title("Which [teal]modules[/] do you want to add?")
                .Required()
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
                .AddChoices(await SearchWeavlyPackagesAsync(workingDir, installedPackages, ct))
                .ShowAsync(AnsiConsole.Console, ct),
        ];

        foreach (var module in selectedModules)
        {
            await Runner
                .WithMessage($"Adding module [teal]{module}[/]...\n")
                .RunAsync(Dotnet.Custom($"package add {module} --project ./{projectName}/{projectName}.csproj"), ct);
        }

        await UpdateProgramBootstrapperAsync(projectName, selectedModules, ct);
    }

    private static async Task<string> SelectProjectAsync(Dictionary<string, string> projects, CancellationToken ct)
    {
        return projects.Keys.Count == 1
            ? projects.Keys.First()
            : await new SelectionPrompt<string>()
                .Title("Where do you want to add the [teal]module[/]?")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
                .AddChoices(projects.Keys)
                .ShowAsync(AnsiConsole.Console, ct);
    }

    [Serializable]
    public class Settings : CommandSettings
    {
        [CommandOption("-p|--project <name>")]
        [Description("Project name")]
        public string? ProjectName { get; init; }
    }
}

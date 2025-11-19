using System.ComponentModel;
using System.Text;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Weavly.Cli.Commands.Module;

[Description("Add database migrations for selected providers to a module")]
public class MigrateCommand : InterruptibleAsyncCommand<MigrateCommand.Settings>
{
    public class Settings : CommandSettings { }

    public override async Task HandleAsync(CommandContext commandContext, Settings settings, CancellationToken ct)
    {
        var modules = GetRelevantProjects().ToDictionary(ExtractFileNameWithoutExtension, f => f);

        var selectedModule = await new SelectionPrompt<string>()
            .Title("Which [teal]module[/] do you want to migrate?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
            .AddChoices(modules.Keys)
            .ShowAsync(AnsiConsole.Console, ct);

        var contexts = RetrieveModuleContexts(modules[selectedModule])
            .ToDictionary(ExtractFileNameWithoutExtension, f => f);
        var rootContext = contexts.Keys.MinBy(c => c.Length)!;
        contexts.Remove(rootContext);

        var selectedProviders = await new MultiSelectionPrompt<string>()
            .Title("Which [teal]providers[/] do you want to migrate?")
            .Required()
            .PageSize(5)
            .MoreChoicesText("[grey](Move up and down to reveal more modules)[/]")
            .AddChoices(contexts.Keys.Select(c => c.Replace(rootContext, string.Empty)))
            .ShowAsync(AnsiConsole.Console, ct);

        var migrationName = await new TextPrompt<string>("Please enter a migration name:").ShowAsync(
            AnsiConsole.Console,
            ct
        );

        foreach (var provider in selectedProviders)
        {
            var contextName = ExtractFileNameWithoutExtension(contexts[provider + rootContext]);
            var fullContextName = await BuildFullContextName(contexts[contextName], contextName, ct);

            var command = new StringBuilder("ef migrations add")
                .Append($" --project {modules[selectedModule].Replace($".{DirectorySeparator}", string.Empty)}")
                .Append(" --startup-project ./Weavly.Api/Weavly.Api.csproj")
                .Append($" --context {fullContextName.Trim()}")
                .Append($" --configuration Debug {migrationName}")
                .Append($" --output-dir Persistence/Migrations/{provider}")
                .Append($" -- --provider {provider.ToLower()}");

            await Runner
                .WithMessage($"Adding migration for [teal]{contextName}[/]...\n")
                .RunAsync("dotnet", command.ToString(), ct);
        }
    }

    private static string ExtractFileNameWithoutExtension(string fileName)
    {
        var fileNameWithExtension = fileName.Split(DirectorySeparator).Last();

        return fileNameWithExtension[..fileNameWithExtension.LastIndexOf('.')];
    }

    private static async Task<string> BuildFullContextName(string context, string contextName, CancellationToken ct)
    {
        return (await File.ReadAllLinesAsync(context, ct))
                .First(l => l.StartsWith("namespace"))
                .Replace("namespace", string.Empty)
                .Trim(';')
            + "."
            + contextName;
    }

    private static List<string> RetrieveModuleContexts(string selectedModule)
    {
        return
        [
            .. Directory
                .GetFiles(
                    selectedModule[..selectedModule.LastIndexOf(DirectorySeparator)],
                    "*DbContext*",
                    SearchOption.AllDirectories
                )
                .Where(f => !f.Contains("ModelSnapshot")),
        ];
    }

    private static string[] GetRelevantProjects()
    {
        return
        [
            .. Directory
                .GetFiles(".", "*.csproj", SearchOption.AllDirectories)
                .Where(f =>
                    !f.Contains(".Shared")
                    && !f.Contains(".Cli")
                    && !f.Contains(".Sample")
                    && !f.Contains("Weavly.Core")
                    && RetrieveModuleContexts(f).Count != 0
                ),
        ];
    }
}

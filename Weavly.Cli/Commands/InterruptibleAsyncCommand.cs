using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models.Dotnet.Package.List;
using Weavly.Cli.Models.Dotnet.Package.Search;
using Weavly.Cli.Utils;

namespace Weavly.Cli.Commands;

public abstract class InterruptibleAsyncCommand<T> : AsyncCommand<T>
    where T : CommandSettings
{
    protected readonly char DirectorySeparator = Path.DirectorySeparatorChar;

    protected ProcessRunner Runner => ProcessRunner.Instance();

    public override async Task<int> ExecuteAsync(CommandContext context, T settings, CancellationToken ct)
    {
        try
        {
            await HandleAsync(context, settings, ct);

            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine();

            if (e is TaskCanceledException)
            {
                return 0;
            }

            AnsiConsole.WriteLine(e.Message);

            return 1;
        }
    }

    protected abstract Task HandleAsync(CommandContext commandContext, T settings, CancellationToken ct = default);

    protected async Task<IEnumerable<string>> SearchWeavlyPackagesAsync(
        string workingDir,
        IEnumerable<string>? installedPackages = null,
        CancellationToken ct = default
    )
    {
        var result = await Runner
            .InDirectory(workingDir)
            .ParseJsonAsync<Search>("dotnet", "package search weavly --format json", ct);

        var choices =
            result
                ?.SearchResult.SelectMany(source => source.Packages)
                .Where(p => !p.Id.EndsWith(".Cli") && !p.Id.EndsWith(".Shared") && !p.Id.Contains(".Core"))
                .OrderBy(p => p.Id, StringComparer.InvariantCulture)
                .Select(p => p.Id)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
            ?? [];

        if (installedPackages is not null)
        {
            return choices
                .Where(c => !installedPackages.Contains(c, StringComparer.OrdinalIgnoreCase))
                .OrderBy(c => c, StringComparer.InvariantCulture);
        }

        return choices;
    }

    protected async Task<IEnumerable<string>> ListWeavlyPackagesAsync(
        string projectName,
        string workingDir = ".",
        CancellationToken ct = default
    )
    {
        var result = await Runner
            .InDirectory(workingDir)
            .ParseJsonAsync<List>("dotnet", $"package list --format json --project {projectName}", ct);

        var choices =
            result
                ?.Projects.First()
                .Frameworks.First()
                .Packages.OrderBy(p => p.Id, StringComparer.InvariantCulture)
                .Select(p => p.Id)
            ?? [];

        return choices;
    }

    protected string ExtractFileNameWithoutExtension(string fileName)
    {
        var fileNameWithExtension = fileName.Split(DirectorySeparator).Last();

        return fileNameWithExtension[..fileNameWithExtension.LastIndexOf('.')];
    }

    protected static string[] GetRelevantProjects()
    {
        return [.. Directory.GetFiles(".", "*.csproj", SearchOption.AllDirectories)];
    }

    protected static async Task UpdateProgramBootstrapperAsync(
        string projectName,
        List<string> selectedModules,
        CancellationToken ct
    )
    {
        AnsiConsole.Write(new Markup($"Updating [teal]{projectName}/Program.cs[/]...\n"));

        var programFilePath = Path.Combine(projectName, "Program.cs");

        var file = await File.ReadAllTextAsync(programFilePath, ct);

        ModuleManagementHelper.UpdateUsings(selectedModules, ref file);
        ModuleManagementHelper.UpdateBuilderSetup(selectedModules, ref file);

        await File.WriteAllTextAsync(programFilePath, file, ct);
    }
}

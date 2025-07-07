using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models;
using Weavly.Cli.Utils;

namespace Weavly.Cli.Commands;

public abstract class InterruptibleAsyncCommand<T> : AsyncCommand<T>
    where T : CommandSettings
{
    protected static readonly char DirectorySeparator = Path.DirectorySeparatorChar;

    private readonly CancellationTokenSource TokenSource = new();
    protected CancellationToken Token => TokenSource.Token;

    protected InterruptibleAsyncCommand()
    {
        Console.CancelKeyPress += (_, args) =>
        {
            args.Cancel = true;
            TokenSource.Cancel();
        };
    }

    public override async Task<int> ExecuteAsync(CommandContext context, T settings)
    {
        try
        {
            await HandleAsync(context, settings);

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

    public abstract Task HandleAsync(CommandContext commandContext, T settings);

    public ProcessRunner Runner => ProcessRunner.Instance(TokenSource);

    protected async Task<IEnumerable<string>> GetAvailableWeavlyPackages(string workingDir)
    {
        var result = await Runner
            .InDirectory(workingDir)
            .ParseJsonAsync<PackageSearchData>("dotnet", $"package search weavly --format json --verbosity detailed");

        var choices =
            result
                ?.SearchResult.First()
                .Packages.Where(p => !p.Id.EndsWith(".Cli") && !p.Id.EndsWith(".Shared") && !p.Id.Contains(".Core"))
                .OrderBy(p => p.Id)
                .Select(p => p.Id) ?? [];
        return choices;
    }
}

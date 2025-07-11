using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Utils;

namespace Weavly.Cli.Commands;

public abstract class InterruptibleAsyncCommand : AsyncCommand
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

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            await HandleAsync(context);

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

    public abstract Task HandleAsync(CommandContext commandContext);

    public ProcessRunner Runner => ProcessRunner.Instance(TokenSource);
}

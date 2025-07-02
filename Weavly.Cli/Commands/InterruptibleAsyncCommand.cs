using Weavly.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Weavly.Cli.Commands;

public abstract class InterruptibleAsyncCommand : AsyncCommand
{
    protected static readonly char DirectorySeparator = Path.DirectorySeparatorChar;

    private readonly CancellationTokenSource TokenSource = new();
    protected CancellationToken Token => TokenSource.Token;

    public InterruptibleAsyncCommand()
    {
        Console.CancelKeyPress += (_, args) =>
        {
            args.Cancel = true;
            TokenSource.Cancel();
        };
    }

    public override async Task<int> ExecuteAsync(CommandContext commandContext)
    {
        try
        {
            await HandleAsync(commandContext);

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

public abstract class InterruptibleAsyncCommand<T> : AsyncCommand<T>
    where T : CommandSettings
{
    protected static readonly char DirectorySeparator = Path.DirectorySeparatorChar;

    private readonly CancellationTokenSource TokenSource = new();
    protected CancellationToken Token => TokenSource.Token;

    public InterruptibleAsyncCommand()
    {
        Console.CancelKeyPress += (_, args) =>
        {
            args.Cancel = true;
            TokenSource.Cancel();
        };
    }

    public override async Task<int> ExecuteAsync(CommandContext commandContext, T settings)
    {
        try
        {
            await HandleAsync(commandContext, settings);

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
}

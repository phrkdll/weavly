using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Weavly.Cli.Utils;

public class ProcessRunner
{
    private readonly CancellationTokenSource tokenSource;
    private IRenderable? message;
    private string? workingDirectory;

    private ProcessRunner(CancellationTokenSource tokenSource)
    {
        this.tokenSource = tokenSource;
    }

    public static ProcessRunner Instance(CancellationTokenSource tokenSource) => new(tokenSource);

    public ProcessRunner WithMessage(string message)
    {
        this.message = new Markup(message);
        return this;
    }

    public ProcessRunner InDirectory(string workingDirectory)
    {
        this.workingDirectory = workingDirectory;
        return this;
    }

    public async Task RunAsync(string fileName, string arguments)
    {
        if (message != null)
        {
            AnsiConsole.Write(message);
        }

        var process =
            Process.Start(
                new ProcessStartInfo(fileName, arguments)
                {
                    WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
                }
            ) ?? throw new InvalidOperationException("Failed to start process");

        await process.WaitForExitAsync(tokenSource.Token);
    }
}

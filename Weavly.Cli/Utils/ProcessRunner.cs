using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Weavly.Cli.Utils;

public class ProcessRunner
{
    private readonly CancellationTokenSource tokenSource;
    private IRenderable? message;
    private string? workingDirectory;

    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
    };

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

    private Process CreateProcess(string fileName, string arguments, bool redirectOutput = false)
    {
        return Process.Start(
                new ProcessStartInfo(fileName, arguments)
                {
                    WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
                    RedirectStandardOutput = redirectOutput,
                }
            ) ?? throw new InvalidOperationException("Failed to start process");
    }

    public async Task<string> RunAsync(string fileName, string arguments)
    {
        if (message != null)
        {
            AnsiConsole.Write(message);
        }

        Process process = CreateProcess(fileName, arguments, true);

        return await process.StandardOutput.ReadToEndAsync(tokenSource.Token);
    }

    public async Task<T?> ParseJsonAsync<T>(string fileName, string arguments)
    {
        if (message != null)
        {
            AnsiConsole.Write(message);
        }

        Process process = CreateProcess(fileName, arguments, true);

        var output = await process.StandardOutput.ReadToEndAsync(tokenSource.Token);

        return JsonSerializer.Deserialize<T>(output, jsonSerializerOptions);
    }
}

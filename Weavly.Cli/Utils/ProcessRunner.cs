using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using Spectre.Console.Rendering;
using Weavly.Cli.Models.ProcessRunner;

namespace Weavly.Cli.Utils;

public class ProcessRunner
{
    private IRenderable? message;
    private string? workingDirectory;

    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
    };

    public static ProcessRunner Instance() => new();

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

    public async Task<string> RunAsync(ProcessRunnerCommand command, CancellationToken ct)
    {
        if (message != null)
        {
            AnsiConsole.Write(message);
        }

        return await CreateProcess(command.Command, command.Arguments, true).StandardOutput.ReadToEndAsync(ct);
    }

    public async Task<T?> ParseJsonAsync<T>(string fileName, string arguments, CancellationToken ct)
    {
        if (message != null)
        {
            AnsiConsole.Write(message);
        }

        Process process = CreateProcess(fileName, arguments, true);

        var output = await process.StandardOutput.ReadToEndAsync(ct);

        return JsonSerializer.Deserialize<T>(output, jsonSerializerOptions);
    }
}

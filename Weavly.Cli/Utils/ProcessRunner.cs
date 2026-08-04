using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using Spectre.Console.Rendering;
using Weavly.Cli.Models.ProcessRunner;

namespace Weavly.Cli.Utils;

public class ProcessRunner
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    private IRenderable? renderableMessage;
    private string? workingDirectory;

    public static ProcessRunner Instance()
    {
        return new ProcessRunner();
    }

    public ProcessRunner WithMessage(string message)
    {
        this.renderableMessage = new Markup(message);
        return this;
    }

    public ProcessRunner InDirectory(string directory)
    {
        this.workingDirectory = directory;
        return this;
    }

    private Process CreateProcess(string fileName, string arguments, bool redirectOutput = false)
    {
        return Process.Start(
            new ProcessStartInfo(fileName, arguments)
            {
                WorkingDirectory = this.workingDirectory ?? Directory.GetCurrentDirectory(),
                RedirectStandardOutput = redirectOutput
            }
        ) ?? throw new InvalidOperationException("Failed to start process");
    }

    public async Task<string> RunAsync(ProcessRunnerCommand command, CancellationToken ct = default)
    {
        if (this.renderableMessage != null)
        {
            AnsiConsole.Write(this.renderableMessage);
        }

        return await CreateProcess(command.Command, command.Arguments, true).StandardOutput.ReadToEndAsync(ct);
    }

    public async Task<T?> ParseJsonAsync<T>(string fileName, string arguments, CancellationToken ct = default)
    {
        if (this.renderableMessage != null)
        {
            AnsiConsole.Write(this.renderableMessage);
        }

        var process = CreateProcess(fileName, arguments, true);

        var output = await process.StandardOutput.ReadToEndAsync(ct);

        return JsonSerializer.Deserialize<T>(output, this.jsonSerializerOptions);
    }
}
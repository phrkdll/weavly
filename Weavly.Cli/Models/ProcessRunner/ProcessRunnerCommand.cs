namespace Weavly.Cli.Models.ProcessRunner;

public abstract class ProcessRunnerCommand(string command, string arguments)
{
    public string Command { get; } = command;

    public string Arguments { get; } = arguments;
}

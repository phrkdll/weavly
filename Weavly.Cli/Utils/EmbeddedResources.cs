using System.Reflection;

namespace Weavly.Cli.Utils;

public static class EmbeddedResources
{
    public static string GetTemplate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        var files = Directory.GetFiles(Path.Combine(assemblyPath, "Templates"));

        var resourceName =
            files.FirstOrDefault(r => r.EndsWith(name))
            ?? throw new InvalidOperationException($"Resource '{name}' not found.");

        using var stream =
            new StreamReader(resourceName)
            ?? throw new InvalidOperationException($"Resource '{resourceName}' not found.");

        return stream.ReadToEnd();
    }
}

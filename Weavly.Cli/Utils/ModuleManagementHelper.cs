using System.Text;
using System.Text.RegularExpressions;

namespace Weavly.Cli.Utils;

public static partial class ModuleManagementHelper
{
    [GeneratedRegex(@"(.*app.Map.*;\s*app)")]
    private static partial Regex RemoveEndpointPattern();

    [GeneratedRegex("(.*app.Run.*;)")]
    private static partial Regex AddAppSetupPattern();

    [GeneratedRegex("(var builder.*;)")]
    private static partial Regex AddBuilderSetupPattern();

    [GeneratedRegex(@"(.*\.AddModule<[^>]+>\(\))(?=\s*\.Build\(\))")]
    private static partial Regex UpdateBuilderSetupPattern();

    [GeneratedRegex(@"(using Weavly.*;)+(\s\s)")]
    private static partial Regex UpdateUsingsPattern();

    public static void RemoveEndpointMappings(ref string file)
    {
        const string replacement = "app";

        file = RemoveEndpointPattern().Replace(file, replacement);
    }

    public static void AddAppSetup(ref string file)
    {
        const string replacement = "app.UseWeavly();\n\n$1";

        file = AddAppSetupPattern().Replace(file, replacement);
    }

    public static void AddBuilderSetup(List<string> selectedModules, ref string file)
    {
        var setupBuilder = new StringBuilder("$1\n\nbuilder.AddWeavly()\n");

        foreach (var module in selectedModules)
        {
            AppendModule(setupBuilder, module);
        }

        setupBuilder.AppendLine("    .Build();");

        file = AddBuilderSetupPattern().Replace(file, setupBuilder.ToString());
    }

    public static void UpdateBuilderSetup(List<string> selectedModules, ref string file)
    {
        var setupBuilder = new StringBuilder("$1\n");

        foreach (var module in selectedModules)
        {
            AppendModule(setupBuilder, module);
        }

        file = UpdateBuilderSetupPattern().Replace(file, setupBuilder.ToString());
    }

    private static void AppendModule(StringBuilder setupBuilder, string module)
    {
        var moduleName = module.Replace("Weavly.", string.Empty).Split('.')[0];
        setupBuilder.AppendLine($"    .AddModule<{moduleName}Module>()");
    }

    public static void AddUsings(List<string> selectedModules, ref string file)
    {
        var usingBuilder = new StringBuilder();

        foreach (var module in selectedModules)
        {
            usingBuilder.AppendLine($"using {module};");
        }

        usingBuilder.AppendLine();

        file = $"{usingBuilder}{file}";
    }

    public static void UpdateUsings(List<string> selectedModules, ref string file)
    {
        var usingBuilder = new StringBuilder("$1\n");

        foreach (var module in selectedModules)
        {
            usingBuilder.AppendLine($"using {module};");
        }

        usingBuilder.AppendLine();

        file = UpdateUsingsPattern().Replace(file, usingBuilder.ToString());
    }
}

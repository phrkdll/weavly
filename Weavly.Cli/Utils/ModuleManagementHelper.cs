using System.Text;
using System.Text.RegularExpressions;

namespace Weavly.Cli.Utils;

public static class ModuleManagementHelper
{
    public static void RemoveEndpointMappings(ref string file)
    {
        string pattern = @"(.*app.Map.*;\s*app)";
        string replacement = "app";

        file = Regex.Replace(file, pattern, replacement);
    }

    public static void AddAppSetup(ref string file)
    {
        string pattern = @"(.*app.Run.*;)";
        string replacement = $"app.UseWeavly();\n\n$1";

        file = Regex.Replace(file, pattern, replacement);
    }

    public static void AddBuilderSetup(List<string> selectedModules, ref string file)
    {
        var setupPattern = @"(var builder.*;)";
        var setupBuilder = new StringBuilder("$1\n\nbuilder.AddWeavly()\n");

        foreach (var module in selectedModules)
        {
            AppendModule(setupBuilder, module);
        }
        setupBuilder.AppendLine("    .Build();");

        file = Regex.Replace(file, setupPattern, setupBuilder.ToString());
    }

    public static void UpdateBuilderSetup(List<string> selectedModules, ref string file)
    {
        var setupPattern = @"(.*\.AddModule<[^>]+>\(\))(?=\s*\.Build\(\))";
        var setupBuilder = new StringBuilder("$1\n");

        foreach (var module in selectedModules)
        {
            AppendModule(setupBuilder, module);
        }

        file = Regex.Replace(file, setupPattern, setupBuilder.ToString());
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
        var usingPattern = @"(using Weavly.*;)+(\s\s)";
        var usingBuilder = new StringBuilder("$1\n");

        foreach (var module in selectedModules)
        {
            usingBuilder.AppendLine($"using {module};");
        }
        usingBuilder.AppendLine();

        file = Regex.Replace(file, usingPattern, usingBuilder.ToString());
    }
}

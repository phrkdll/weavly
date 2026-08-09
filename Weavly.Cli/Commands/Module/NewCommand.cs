using System.ComponentModel;
using Fluid;
using Spectre.Console;
using Spectre.Console.Cli;
using Weavly.Cli.Models;
using Weavly.Cli.Models.ProcessRunner;
using Weavly.Cli.Utils;

namespace Weavly.Cli.Commands.Module;

[Description("Initializes a new custom module project for the current solution")]
public class NewCommand : InterruptibleAsyncCommand<NewCommand.Settings>
{
    private readonly WeavlyModule coreModule = WeavlyModule.New("Core", "Weavly");

    private readonly IEnumerable<string> xunitPackages =
    [
        "coverlet.collector",
        "Microsoft.NET.Test.Sdk",
        "xunit",
        "xunit.runner.visualstudio",
    ];

    protected override async Task HandleAsync(
        CommandContext commandContext,
        Settings settings,
        CancellationToken ct = default
    )
    {
        var solutionName =
            Directory.GetFiles(".", "*.sln*").Select(Path.GetFileNameWithoutExtension).FirstOrDefault()
            ?? throw new InvalidOperationException("No solution file found in the current directory.");

        var moduleName =
            settings.ModuleName
            ?? await new TextPrompt<string>("Please enter a module name:").ShowAsync(AnsiConsole.Console, ct);

        var module = WeavlyModule.New(moduleName, solutionName);

        AnsiConsole.Write(new Markup($"Creating module [teal]{module.FullName}[/]...\n"));
        await Runner
            .WithMessage($"Adding project [teal]{module.Main}[/]...\n")
            .RunAsync(Dotnet.AddProject("classlib", module.Main), ct);

        await Runner
            .WithMessage($"Adding project [teal]{module.Shared}[/]...\n")
            .RunAsync(Dotnet.AddProject("classlib", module.Shared), ct);

        await Runner
            .WithMessage($"Adding project [teal]{module.Tests}[/]...\n")
            .RunAsync(Dotnet.AddProject("classlib", module.Tests), ct);

        await Runner
            .WithMessage("Adding project/package references...\n")
            .RunAsync(Dotnet.AddReference(module.Main, module.Shared.Folder), ct);
        await Runner.RunAsync(Dotnet.AddReference(module.Tests, module.Main.Folder, module.Shared.Folder), ct);

        foreach (var pkg in xunitPackages)
        {
            await Runner.RunAsync(Dotnet.AddPackage(module.Tests, pkg), ct);
        }

        if (solutionName == coreModule.Solution)
        {
            await Runner.RunAsync(Dotnet.AddReference(module.Main, coreModule.Main.Folder), ct);
            await Runner.RunAsync(Dotnet.AddReference(module.Shared, coreModule.Shared.Folder), ct);
            await Runner.RunAsync(Dotnet.AddReference(module.Shared, coreModule.Tests.Folder + ".Shared"), ct);
        }
        else
        {
            await Runner.RunAsync(Dotnet.AddPackage(module.Main, coreModule.Main.FullName), ct);
            await Runner.RunAsync(Dotnet.AddPackage(module.Shared, coreModule.Shared.FullName), ct);
            await Runner.RunAsync(Dotnet.AddPackage(module.Shared, coreModule.Tests.FullName + ".Shared"), ct);
        }

        const string defaultClassFile = "Class1.cs";
        File.Delete(Path.Combine(module.Main.Folder, defaultClassFile));
        File.Delete(Path.Combine(module.Shared.Folder, defaultClassFile));
        File.Delete(Path.Combine(module.Tests.Folder, defaultClassFile));

        await RenderTemplateToFileAsync("Module", module, module.Main);
        await RenderTemplateToFileAsync("Repository", module, module.Main, "Persistence");
        await RenderTemplateToFileAsync("HandlerTests", module, module.Tests);

        await Runner
            .WithMessage("Adding projects to solution...\n")
            .RunAsync(Dotnet.AddToSolution(module.Main, module.Shared, module.Tests), ct);
    }

    private static async Task RenderTemplateToFileAsync(
        string templateName,
        WeavlyModule module,
        WeavlyProject project,
        string subPath = ""
    )
    {
        var parser = new FluidParser();
        if (parser.TryParse(EmbeddedResources.GetTemplate(templateName + ".cs.template"), out var template))
        {
            if (!Directory.Exists(Path.Combine(project.Folder, subPath)))
            {
                Directory.CreateDirectory(Path.Combine(project.Folder, subPath));
            }

            await using var writer = new StreamWriter(
                Path.Combine(project.Folder, subPath, $"{module.Name}{templateName}.cs"),
                false
            );

            var context = new TemplateContext(module);
            await template.RenderAsync(writer, context);
        }
    }

    [Serializable]
    public class Settings : CommandSettings
    {
        [CommandOption("-n|--name <name>")]
        [Description("Module name")]
        public string? ModuleName { get; init; }
    }
}

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Implementation;

internal sealed class WeavlyModuleInitializer : IHostedService
{
    private List<IWeavlyModule> orderedModules;
    private readonly HashSet<IWeavlyModule> visited = [];
    private readonly HashSet<IWeavlyModule> visiting = [];

    private readonly IServiceScopeFactory scopeFactory;

    public WeavlyModuleInitializer(IReadOnlyList<IWeavlyModule> modules, IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;

        OrderByInitializationDependencies(modules);
    }

    [MemberNotNull(nameof(orderedModules))]
    private void OrderByInitializationDependencies(IReadOnlyList<IWeavlyModule> modules)
    {
        var modulesByName = modules.ToDictionary(x => x.GetType().Name, StringComparer.Ordinal);
        orderedModules = new List<IWeavlyModule>(modules.Count);

        foreach (var module in modules)
        {
            Visit(module, modulesByName);
        }
    }

    private void Visit(IWeavlyModule module, Dictionary<string, IWeavlyModule> modulesByName)
    {
        if (visited.Contains(module))
        {
            return;
        }

        if (!visiting.Add(module))
        {
            throw new InvalidOperationException(
                $"A module initialization dependency cycle includes {module.GetType().Name}."
            );
        }

        foreach (var dependencyName in module.InitializationDependencies)
        {
            if (!modulesByName.TryGetValue(dependencyName, out var dependency))
            {
                throw new InvalidOperationException(
                    $"Module {module.GetType().Name} requires unregistered module {dependencyName}."
                );
            }

            Visit(dependency, modulesByName);
        }

        visiting.Remove(module);
        visited.Add(module);
        orderedModules.Add(module);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        foreach (var module in orderedModules)
        {
            await module.InitializeAsync(bus);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Implementation;

public sealed class WeavlyApplicationBuilder(IHostApplicationBuilder builder) : IWeavlyApplicationBuilder
{
    private readonly List<IWeavlyModule> modules = [];

    public IEnumerable<IWeavlyModule> Modules => modules;

    public IWeavlyApplicationBuilder AddModule<T>()
        where T : IWeavlyModule
    {
        if (modules.All(x => x.GetType() != typeof(T)) && Activator.CreateInstance<T>() is IWeavlyModule module)
        {
            modules.Add(module);
        }

        return this;
    }

    public void Build()
    {
        foreach (var module in modules)
        {
            module.Configure(builder);
        }

        var assemblies = modules.Select(module => module.GetType().Assembly).ToArray();

        builder.UseWolverine(x =>
        {
            x.Policies.MessageExecutionLogLevel(LogLevel.None);
            x.Policies.MessageSuccessLogLevel(LogLevel.None);

            x.Discovery.DisableConventionalDiscovery();
            x.Discovery.CustomizeMessageDiscovery(m => m.Includes.Implements<IWeavlyCommand>());
            x.Discovery.CustomizeHandlerDiscovery(h => h.Includes.Implements<IWeavlyHandler>());

            foreach (var assembly in assemblies)
            {
                x.Discovery.IncludeAssembly(assembly);
            }
        });

        builder.Services.AddSingleton<IHostedService>(serviceProvider =>
            new WeavlyModuleInitializer(modules, serviceProvider.GetRequiredService<IServiceScopeFactory>())
        );

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenApi();
        }
    }
}
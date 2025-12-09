using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Implementation;

public sealed class WeavlyApplicationBuilder(IHostApplicationBuilder builder) : IWeavlyApplicationBuilder
{
    private readonly HashSet<IWeavlyModule> modules = [];

    public IEnumerable<IWeavlyModule> Modules => modules;

    public IWeavlyApplicationBuilder AddModule<T>()
        where T : IWeavlyModule
    {
        if (Activator.CreateInstance<T>() is IWeavlyModule module)
        {
            modules.Add(module);
        }

        return this;
    }

    public void Build()
    {
        foreach (var module in Modules)
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

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenApi();
        }
    }
}

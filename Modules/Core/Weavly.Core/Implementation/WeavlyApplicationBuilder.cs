using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Implementation;

internal sealed class WeavlyApplicationBuilder(WebApplicationBuilder builder) : IWeavlyApplicationBuilder
{
    private readonly List<IWeavlyModule> _modules = [];

    public IEnumerable<IWeavlyModule> Modules => _modules;

    public IWeavlyApplicationBuilder AddModule<T>()
        where T : IWeavlyModule
    {
        if (Activator.CreateInstance<T>() is IWeavlyModule module)
        {
            _modules.Add(module);
        }

        return this;
    }

    public void Build()
    {
        foreach (var module in Modules)
        {
            module.Configure(builder);
        }

        var assemblies = _modules.Select(module => module.GetType().Assembly).ToArray();

        builder.Services.AddFastEndpoints(o => o.Assemblies = assemblies);

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.SwaggerDocument();
        }
    }
}

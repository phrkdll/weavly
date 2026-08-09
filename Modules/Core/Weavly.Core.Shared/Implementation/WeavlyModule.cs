using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Seeding;
using Wolverine;

namespace Weavly.Core.Shared.Implementation;

public abstract class WeavlyModule : IWeavlyModule
{
    public virtual void Configure(IHostApplicationBuilder builder) { }

    public virtual void Use(WebApplication app)
    {
        var numberOfEndpoints = app.MapEndpoints(this);

        app.Logger.LogInformation(
            "Registered module {ModuleName} with {NumberOfEndpoints} endpoints",
            GetType().Namespace,
            numberOfEndpoints
        );
    }

    public virtual async Task InitializeAsync(IMessageBus bus)
    {
        var instances = GetType().Assembly.DefinedTypes.Where(x => typeof(IWeavlySeed).IsAssignableFrom(x));

        foreach (var instance in instances.Select(t => Activator.CreateInstance(t, true) as IWeavlySeed).ToArray())
        {
            if (instance is not null)
            {
                await instance.SeedAsync(bus);
            }
        }
    }
}

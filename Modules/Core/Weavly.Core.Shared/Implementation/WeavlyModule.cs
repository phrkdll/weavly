using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Seeding;
using Wolverine;

namespace Weavly.Core.Shared.Implementation;

public abstract class WeavlyModule : IWeavlyModule
{
    public virtual IReadOnlyCollection<string> InitializationDependencies => [];

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
        var instances = GetType()
            .Assembly.DefinedTypes.Where(x =>
                !x.IsAbstract && !x.IsInterface && typeof(IWeavlySeed).IsAssignableFrom(x)
            )
            .OrderBy(x => x.FullName, StringComparer.Ordinal);

        foreach (var seedType in instances)
        {
            if (Activator.CreateInstance(seedType, true) is not IWeavlySeed seed)
            {
                throw new InvalidOperationException($"Could not create seed {seedType.FullName}.");
            }

            var result = await seed.SeedAsync(bus);
            if (result is Failure failure)
            {
                throw new InvalidOperationException(
                    $"Seed {seedType.Name} failed: {failure.Message}",
                    failure.Exception
                );
            }
        }
    }
}

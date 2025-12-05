using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weavly.Core.Shared.Contracts;
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

    public virtual Task InitializeAsync(IMessageBus bus)
    {
        return Task.CompletedTask;
    }

    public virtual Type[] DbContextTypes => [];
}

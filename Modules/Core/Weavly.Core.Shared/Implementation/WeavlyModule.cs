using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Shared.Implementation;

public abstract class WeavlyModule : IWeavlyModule
{
    public virtual void Configure(IHostApplicationBuilder builder) { }

    public virtual void Use(WebApplication app)
    {
        app.Logger.LogInformation("Registered module {ModuleName}", GetType().Namespace);
    }

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Type[] DbContextTypes => [];
}

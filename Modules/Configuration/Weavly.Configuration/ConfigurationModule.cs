using Microsoft.Extensions.Hosting;
using Weavly.Configuration.Persistence;
using Weavly.Core;

namespace Weavly.Configuration;

public sealed class ConfigurationModule : WeavlyModule
{
    public override void Configure(IHostApplicationBuilder builder)
    {
        builder.AddWeavlyModuleDbContext<ConfigurationModule, ConfigurationDbContext>();

        base.Configure(builder);
    }

    public override Type[] DbContextTypes => [typeof(ConfigurationDbContext)];
}

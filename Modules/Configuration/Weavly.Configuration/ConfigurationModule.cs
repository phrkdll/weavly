using Microsoft.AspNetCore.Builder;
using Weavly.Configuration.Persistence;
using Weavly.Core;

namespace Weavly.Configuration;

public sealed class ConfigurationModule : WeavlyModule
{
    public override void Configure(WebApplicationBuilder builder)
    {
        builder.AddWeavlyModuleDbContext<ConfigurationModule, ConfigurationDbContext>();

        base.Configure(builder);
    }

    public override Type[] DbContextTypes => [typeof(ConfigurationDbContext)];
}

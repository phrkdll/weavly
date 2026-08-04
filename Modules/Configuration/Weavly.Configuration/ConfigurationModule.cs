using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weavly.Configuration.Persistence;

namespace Weavly.Configuration;

[ExcludeFromCodeCoverage]
public sealed class ConfigurationModule : WeavlyModule
{
    public override void Configure(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ConfigurationRepository>();

        base.Configure(builder);
    }
}
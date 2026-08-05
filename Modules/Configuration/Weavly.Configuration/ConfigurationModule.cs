using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weavly.Configuration.Features.UpdateConfiguration;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared.Features.UpdateConfiguration;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration;

[ExcludeFromCodeCoverage]
public sealed class ConfigurationModule : WeavlyModule
{
    public override void Configure(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ConfigurationRepository>();

        builder.Services.AddScoped<IValidator<UpdateConfigurationCommand>, UpdateConfigurationCommandValidator>();

        base.Configure(builder);
    }
}

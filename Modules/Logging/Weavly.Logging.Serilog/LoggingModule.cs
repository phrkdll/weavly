using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Weavly.Logging.Serilog;

[ExcludeFromCodeCoverage]
public sealed class LoggingModule : WeavlyModule
{
    public override void Configure(IHostApplicationBuilder builder)
    {
        ((WebApplicationBuilder)builder).Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true);
            }
        );

        base.Configure(builder);
    }

    public override void Use(WebApplication app)
    {
        app.UseSerilogRequestLogging();

        base.Use(app);
    }
}
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Weavly.Logging.Serilog;

public sealed class LoggingModule : WeavlyModule
{
    public override void Configure(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            (_, configuration) =>
            {
                configuration.WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true);
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

using Weavly.Auth;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Configuration;
using Weavly.Core;
using Weavly.Mail;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (_, configuration) =>
    {
        configuration.WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true);
    }
);

builder
    .AddWeavly()
    .AddModule<CoreModule<AppUserId>>()
    .AddModule<ConfigurationModule>()
    .AddModule<AuthModule>()
    .AddModule<MailModule>()
    .Build();

builder.Services.AddCors();

var app = builder.Build();
app.UseCors(cors =>
{
    cors.AllowAnyHeader();
    cors.AllowAnyMethod();
    cors.AllowAnyOrigin();
});

app.UseWeavlyModules();

app.UseSerilogRequestLogging();

await app.RunAsync();

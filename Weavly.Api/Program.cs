using Weavly.Auth;
using Weavly.Configuration;
using Weavly.Core;
using Weavly.Logging.Serilog;
using Weavly.Mail;
using Weavly.Messages;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder
    .AddWeavly()
    .AddModule<AuthModule>()
    .AddModule<CoreModule>()
    .AddModule<ConfigurationModule>()
    .AddModule<MailModule>()
    .AddModule<MessagesModule>()
    .AddModule<LoggingModule>()
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

await app.RunAsync();

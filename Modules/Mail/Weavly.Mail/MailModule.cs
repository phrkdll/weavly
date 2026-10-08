using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weavly.Mail.Implementation;
using Weavly.Mail.Persistence;
using Weavly.Mail.Shared.Contracts;

namespace Weavly.Mail;

public class MailModule : WeavlyModule
{
    public override IReadOnlyCollection<string> InitializationDependencies => ["ConfigurationModule"];

    public override void Configure(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMailService, MailService>();
        builder.Services.AddScoped<MailRepository>();

        base.Configure(builder);
    }

    public static class ConfigCategory
    {
        public const string General = "General";
    }
}

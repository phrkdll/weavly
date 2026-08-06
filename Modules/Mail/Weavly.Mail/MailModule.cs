using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weavly.Configuration.Shared.Features.CreateConfiguration;
using Weavly.Mail.Implementation;
using Weavly.Mail.Shared.Contracts;
using Wolverine;

namespace Weavly.Mail;

public class MailModule : WeavlyModule
{
    public override void Configure(IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMailService, MailService>();

        base.Configure(builder);
    }

    public override async Task InitializeAsync(IMessageBus bus)
    {
        CreateConfigurationCommand[] configurationItems =
        [
            CreateConfigurationCommand.Create<MailModule>(
                "DefaultSender",
                "no-reply@weavly.com",
                ConfigCategory.General
            ),
            CreateConfigurationCommand.Create<MailModule>("SmtpHost", "localhost", ConfigCategory.General),
            CreateConfigurationCommand.Create<MailModule>("SmtpPort", 1025, ConfigCategory.General),
            CreateConfigurationCommand.Create<MailModule>("EnableSsl", false, ConfigCategory.General),
            CreateConfigurationCommand.Create<MailModule>("SmtpUser", string.Empty, ConfigCategory.General),
            CreateConfigurationCommand.Create<MailModule>("SmtpPassword", string.Empty, ConfigCategory.General),
        ];

        foreach (var item in configurationItems)
        {
            await bus.InvokeAsync<Result>(item);
        }

        await base.InitializeAsync(bus);
    }

    public static class ConfigCategory
    {
        public const string General = "General";
    }
}

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
            CreateConfigurationCommand.Create<MailModule>("DefaultSender", "no-reply@weavly.com"),
            CreateConfigurationCommand.Create<MailModule>("SmtpHost", "smtp4dev"),
            CreateConfigurationCommand.Create<MailModule>("SmtpPort", 25),
            CreateConfigurationCommand.Create<MailModule>("EnableSsl", false),
            CreateConfigurationCommand.Create<MailModule>("SmtpUser", string.Empty),
            CreateConfigurationCommand.Create<MailModule>("SmtpPassword", string.Empty),
        ];

        foreach (var item in configurationItems)
        {
            await bus.InvokeAsync<Result>(item);
        }

        await base.InitializeAsync(bus);
    }
}

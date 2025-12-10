using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Mail.Shared.Contracts;
using Wolverine;

namespace Weavly.Mail.Implementation;

public sealed class MailService(IConfiguration configuration, ILogger<MailService> logger, IMessageBus bus)
    : IMailService
{
    public async Task SendEmailAsync(MimeMessage message, CancellationToken ct = default)
    {
        var options =
            await bus.InvokeAsync<Result>(LoadConfigurationCommand.Create<MailModule>(), ct)
            as Success<LoadConfigurationResponse>;

        ArgumentNullException.ThrowIfNull(options);

        var smtpOptions = SmtpOptions.FromConfigurationResponse(options.Data);
        var smtpHostOverride = configuration["MailModule:SmtpHost"];

        try
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(
                smtpHostOverride ?? smtpOptions.SmtpHost,
                smtpOptions.SmtpPort,
                smtpOptions.EnableSsl,
                ct
            );
            await client.AuthenticateAsync(smtpOptions.SmtpUser, smtpOptions.SmtpPassword, ct);

            await client.SendAsync(message, ct);

            await client.DisconnectAsync(true, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending email");
        }
    }
}

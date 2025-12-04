using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Weavly.Configuration.Shared.Features.LoadConfig;
using Weavly.Mail.Shared;

namespace Weavly.Mail.Implementation;

internal sealed class MailService(IConfiguration configuration, ILogger<MailService> logger) : IMailService
{
    public async Task SendEmailAsync(MimeMessage message, CancellationToken ct = default)
    {
        var options =
            await LoadConfigurationCommand.Create<MailModule>().ExecuteAsync(ct) as Success<LoadConfigurationResponse>;
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
            throw;
        }
    }
}

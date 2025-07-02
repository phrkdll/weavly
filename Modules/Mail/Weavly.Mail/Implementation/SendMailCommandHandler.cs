using FastEndpoints;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Weavly.Configuration.Shared.Features.LoadConfig;
using Weavly.Mail.Shared;
using Weavly.Mail.Shared.Triggers;

namespace Weavly.Mail.Implementation;

public sealed class SendMailCommandHandler(IMailService mailService, ILogger<SendMailCommandHandler> logger)
    : ICommandHandler<SendMailCommand, Result>
{
    public async Task<Result> ExecuteAsync(SendMailCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Received {MessageType} message", nameof(SendMailCommand));

        var config = await LoadConfigurationCommand.Create<MailModule>().ExecuteAsync(ct);

        if (config is not Success<LoadConfigurationResponse> success)
        {
            return Failure.Create("Could not load configuration");
        }

        var smtpOptions = SmtpOptions.FromConfigurationResponse(success.Data);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Weavly", smtpOptions.DefaultSender));
        message.To.Add(new MailboxAddress(command.To, command.To));

        message.Subject = command.Subject;

        var textFormat = command.Body.Contains("</") ? TextFormat.Html : TextFormat.Plain;
        message.Body = new TextPart(textFormat) { Text = command.Body };

        await mailService.SendEmailAsync(message, ct);

        return Success.Create(message);
    }
}

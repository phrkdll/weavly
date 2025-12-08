using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Weavly.Configuration.Shared.Features.LoadConfig;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Implementation;
using Weavly.Mail.Shared.Contracts;
using Weavly.Mail.Shared.Features.SendMail;
using Wolverine;

namespace Weavly.Mail.Features.SendMail;

public sealed class SendMailHandler(IMailService mailService, ILogger<SendMailHandler> logger, IMessageBus bus)
    : IWeavlyHandler<SendMailCommand, Result>
{
    public async Task<Result> HandleAsync(SendMailCommand command, CancellationToken ct)
    {
        logger.LogInformation("Received {MessageType} message", nameof(SendMailCommand));

        var config = await bus.InvokeAsync<Result>(LoadConfigurationCommand.Create<MailModule>(), ct);

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

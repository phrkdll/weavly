using Fluid;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Implementation;
using Weavly.Mail.Persistence;
using Weavly.Mail.Shared.Contracts;
using Weavly.Mail.Shared.Features.SendMail;
using Wolverine;

namespace Weavly.Mail.Features.SendMail;

public sealed class SendMailHandler(
    MailRepository repo,
    IMailService mailService,
    ITemplateService templateService,
    ILogger<SendMailHandler> logger,
    IMessageBus bus
) : IWeavlyHandler<SendMailCommand>
{
    public async Task<Result> HandleAsync(SendMailCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Received {MessageType} message", nameof(SendMailCommand));

        if (
            await bus.InvokeAsync<Result>(LoadConfigurationCommand.Create<MailModule>(), ct)
            is not Success<LoadConfigurationResponse> config
        )
        {
            return Result.Failure("Could not load configuration");
        }

        var smtpOptions = SmtpOptions.FromConfigurationResponse(config.Data);

        using var session = await repo.StartSessionAsync(ct);

        var template = await repo.MailTemplates.FindAsync(
            x => x.Module == command.Module && x.Name == command.Name,
            ct
        );
        if (template is null)
        {
            logger.LogWarning("Could not find mail template {Name} for {Module}", command.Name, command.Module);

            return Result.Failure("Could not find mail template");
        }

        var rendered = await templateService.RenderAsync(template.Text, new { BaseUrl = "", Token = "T" });
        if (rendered is not Success<string> body)
        {
            logger.LogWarning("Could not find mail template {Name} for {Module}", command.Name, command.Module);

            return Result.Failure("Could not find mail template");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Weavly", smtpOptions.DefaultSender));
        message.To.Add(new MailboxAddress(command.To, command.To));

        message.Subject = template.Subject;

        var textFormat = body.Data.Contains("</") ? TextFormat.Html : TextFormat.Plain;
        message.Body = new TextPart(textFormat) { Text = body.Data };

        await mailService.SendEmailAsync(message, ct);

        return Result.Success(message);
    }
}

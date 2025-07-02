using MimeKit;

namespace Weavly.Mail.Shared;

public interface IMailService
{
    Task SendEmailAsync(MimeMessage message, CancellationToken ct = default);
}

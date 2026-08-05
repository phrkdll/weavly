using MimeKit;

namespace Weavly.Mail.Shared.Contracts;

public interface IMailService
{
    Task SendEmailAsync(MimeMessage message, CancellationToken ct = default);
}

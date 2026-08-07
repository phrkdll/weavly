using Microsoft.Extensions.Logging;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Models;
using Weavly.Mail.Persistence;
using Weavly.Mail.Shared.Features.CreateMailTemplate;

namespace Weavly.Mail.Features.CreateMailTemplate;

public sealed class CreateMailTemplateHandler(MailRepository repo, ILogger<CreateMailTemplateHandler> logger)
    : IWeavlyHandler<CreateMailTemplateCommand>
{
    public async Task<Result> HandleAsync(CreateMailTemplateCommand command, CancellationToken ct = default)
    {
        try
        {
            using var session = await repo.StartTransactionAsync(ct);

            if (
                await repo.MailTemplates.FindAsync(x => x.Module == command.Module && x.Name == command.Name, ct)
                != null
            )
            {
                logger.LogDebug("Mail template {Module} -> {Name} already exists", command.Module, command.Name);

                return Result.Failure("The mail template already exists");
            }

            logger.LogDebug("Mail template {Module} -> {Name} created", command.Module, command.Name);
            var template = new MailTemplate(command.Module, command.Name, command.Subject, command.Text);
            await repo.MailTemplates.InsertAsync(template, ct);

            await session.CommitTransactionAsync(ct);

            return Result.Success(new CreateMailTemplateResponse(template.Id));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }
}

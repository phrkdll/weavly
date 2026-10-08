using Microsoft.Extensions.Logging;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Models;
using Weavly.Mail.Persistence;
using Weavly.Mail.Shared.Features.SeedMailTemplate;

namespace Weavly.Mail.Features.SeedMailTemplate;

public sealed class SeedMailTemplateHandler(MailRepository repo, ILogger<SeedMailTemplateHandler> logger)
    : IWeavlyHandler<SeedMailTemplateCommand>
{
    public async Task<Result> HandleAsync(SeedMailTemplateCommand command, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            using var session = await repo.StartTransactionAsync(ct);

            var existing = await repo.MailTemplates.FindAsync(
                x => x.Module == command.Module && x.Name == command.Name,
                ct
            );
            if (existing is not null)
            {
                logger.LogDebug("Seeded mail template {Module} -> {Name} already exists", command.Module, command.Name);
                return Result.Success(new SeedMailTemplateResponse(existing.Id, false));
            }

            var template = new MailTemplate(command.Module, command.Name, command.Subject, command.Text);
            await repo.MailTemplates.InsertAsync(template, ct);
            await session.CommitTransactionAsync(ct);

            logger.LogDebug("Seeded mail template {Module} -> {Name}", command.Module, command.Name);
            return Result.Success(new SeedMailTemplateResponse(template.Id, true));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }
}

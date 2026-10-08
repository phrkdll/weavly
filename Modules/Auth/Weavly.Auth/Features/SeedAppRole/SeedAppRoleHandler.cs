using Microsoft.Extensions.Logging;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.SeedAppRole;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.SeedAppRole;

public sealed class SeedAppRoleHandler(AuthRepository repo, ILogger<SeedAppRoleHandler> logger)
    : IWeavlyHandler<SeedAppRoleCommand>
{
    public async Task<Result> HandleAsync(SeedAppRoleCommand command, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            using var session = await repo.StartTransactionAsync(ct);
            var existing = await repo.Roles.FindAsync(x => x.Name == command.Name, ct);
            if (existing is not null)
            {
                logger.LogDebug("Seeded app role {Name} already exists", command.Name);
                return Result.Success(new SeedAppRoleResponse(existing.Id, false));
            }

            var role = AppRole.Create(command.Name);
            await repo.Roles.InsertAsync(role, ct);
            await session.CommitTransactionAsync(ct);

            logger.LogDebug("Seeded app role {Name}", command.Name);
            return Result.Success(new SeedAppRoleResponse(role.Id, true));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }
}

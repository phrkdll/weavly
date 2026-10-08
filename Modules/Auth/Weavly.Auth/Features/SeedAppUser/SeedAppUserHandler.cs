using Microsoft.Extensions.Logging;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.SeedAppUser;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.SeedAppUser;

public sealed class SeedAppUserHandler(AuthRepository repo, ILogger<SeedAppUserHandler> logger)
    : IWeavlyHandler<SeedAppUserCommand>
{
    public async Task<Result> HandleAsync(SeedAppUserCommand command, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            using var session = await repo.StartTransactionAsync(ct);
            var existing = await repo.Users.FindAsync(x => x.Email == command.Email, ct);
            if (existing is not null)
            {
                logger.LogDebug("Seeded app user {Email} already exists", command.Email);
                return Result.Success(new SeedAppUserResponse(existing.Id, false));
            }

            var role = await repo.Roles.FindAsync(x => x.Name == command.InitialRole, ct);
            if (role is null)
            {
                return Result.Failure($"Seed role {command.InitialRole} does not exist");
            }

            var user = AppUser.Create(command.Email, command.UserName, role);
            await repo.Users.InsertAsync(user, ct);
            await session.CommitTransactionAsync(ct);

            logger.LogDebug("Seeded app user {Email}", command.Email);
            return Result.Success(new SeedAppUserResponse(user.Id, true));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }
}

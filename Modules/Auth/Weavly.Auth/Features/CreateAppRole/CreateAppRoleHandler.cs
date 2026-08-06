using Microsoft.Extensions.Logging;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.CreateAppRole;

public sealed class CreateAppRoleHandler(AuthRepository repo, ILogger<CreateAppRoleHandler> logger)
    : IWeavlyHandler<CreateAppRoleCommand>
{
    public async Task<Result> HandleAsync(CreateAppRoleCommand command, CancellationToken ct = default)
    {
        logger.LogDebug("Received {Command}", command);

        try
        {
            using var session = await repo.StartSessionAsync(ct);

            var role = AppRole.Create(command.Name);
            if (await IsRoleNameAvailable(role) is Failure f)
            {
                return f;
            }

            await repo.Roles.InsertAsync(role, ct);
            await session.CommitTransactionAsync(ct);

            return Result.Success(new CreateAppRoleResponse(role.Id));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }

    private async Task<Result> IsRoleNameAvailable(AppRole role)
    {
        var roles = await repo.Roles.FilterAsync(x => x.Name == role.Name);

        return !roles.Any() ? Result.Success() : Result.Failure("Role name is not available.");
    }
}

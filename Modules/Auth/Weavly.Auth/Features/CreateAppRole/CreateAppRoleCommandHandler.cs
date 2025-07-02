using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.CreateAppRole;

namespace Weavly.Auth.Features.CreateAppRole;

public sealed class CreateAppRoleCommandHandler(AuthDbContext dbContext, ILogger<CreateAppRoleCommandHandler> logger)
    : ICommandHandler<CreateAppRoleCommand, Result>
{
    public async Task<Result> ExecuteAsync(CreateAppRoleCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Received {MessageType} message", nameof(CreateAppRoleCommand));

        try
        {
            var role = AppRole.Create(command.Name);
            if (await RoleCanNotBeCreatedAsync(role))
            {
                return Failure.Create("Role can't be created");
            }

            dbContext.Add(role);

            await dbContext.SaveChangesAsync(ct);

            return Success.Create(role.Id);
        }
        catch (Exception e)
        {
            return Failure.Create(e);
        }
    }

    private async Task<bool> RoleCanNotBeCreatedAsync(AppRole? request)
    {
        if (request is null)
        {
            return true;
        }

        return await dbContext.Roles.AnyAsync(x => x.Name == request.Name);
    }
}

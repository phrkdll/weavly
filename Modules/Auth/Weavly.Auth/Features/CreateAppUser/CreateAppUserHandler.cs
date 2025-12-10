using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.CreateAppUser;

public sealed class CreateAppUserHandler(AuthDbContext dbContext, ILogger<CreateAppUserHandler> logger)
    : IWeavlyHandler<CreateAppUserCommand, Result>
{
    public async Task<Result> HandleAsync(CreateAppUserCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Received {MessageType} message", nameof(CreateAppUserCommand));

        try
        {
            if (await UserCanNotBeRegisteredAsync(command))
            {
                return Failure.Create("User can't be registered");
            }

            var role = await dbContext.Roles.SingleOrDefaultAsync(x => x.Name == command.InitialRole, ct);
            if (role is null && !await dbContext.Users.AnyAsync(ct))
            {
                role = AppRole.Create(command.InitialRole);
                dbContext.Roles.Add(role);
            }

            ArgumentNullException.ThrowIfNull(role);
            var user = AppUser.Create(command.Email, command.UserName, role);
            dbContext.Add(user);
            await dbContext.SaveChangesAsync(ct);

            return Success.Create(user.Id);
        }
        catch (Exception e)
        {
            return Failure.Create(e);
        }
    }

    private async Task<bool> UserCanNotBeRegisteredAsync(CreateAppUserCommand? request)
    {
        if (request is null)
        {
            return true;
        }

        return await dbContext.Users.AnyAsync(x => x.Email == request.Email);
    }
}

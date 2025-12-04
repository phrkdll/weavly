using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.CreateAppUser;

namespace Weavly.Auth.Features.CreateAppUser;

public sealed class CreateAppUserCommandHandler(AuthDbContext dbContext, ILogger<CreateAppUserCommandHandler> logger)
    : ICommandHandler<CreateAppUserCommand, Result>
{
    public async Task<Result> ExecuteAsync(CreateAppUserCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Received {MessageType} message", nameof(CreateAppUserCommand));

        try
        {
            if (await UserCanNotBeRegisteredAsync(command))
            {
                return Failure.Create("User can't be registered");
            }

            var role = dbContext.Roles.SingleOrDefault(x => x.Name == command.InitialRole);
            if (role is null && !dbContext.Users.Any())
            {
                role = AppRole.Create(command.InitialRole);
                dbContext.Roles.Add(role);
            }

            ArgumentNullException.ThrowIfNull(role, nameof(role));
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

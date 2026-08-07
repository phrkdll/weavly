using Microsoft.Extensions.Logging;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.CreateAppUser;

public sealed class CreateAppUserHandler(AuthRepository repo, ILogger<CreateAppUserHandler> logger)
    : IWeavlyHandler<CreateAppUserCommand>
{
    public async Task<Result> HandleAsync(CreateAppUserCommand command, CancellationToken ct = default)
    {
        try
        {
            using var session = await repo.StartTransactionAsync(ct);

            if (await IsEmailAddressAvailable(command) is Failure emailInUseFailure)
            {
                logger.LogDebug("Email ({Email}) is already in use.", command.Email);
                return emailInUseFailure;
            }

            var role = await repo.Roles.FindAsync(x => x.Name == command.InitialRole, ct);
            if (role is null)
            {
                role = AppRole.Create(command.InitialRole);
                await repo.Roles.InsertAsync(role, ct);
            }

            var user = AppUser.Create(command.Email, command.UserName, role);
            await repo.Users.InsertAsync(user, ct);

            await session.CommitTransactionAsync(ct);

            return Result.Success(new CreateAppUserResponse(user.Id));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }

    private async Task<Result> IsEmailAddressAvailable(CreateAppUserCommand request)
    {
        var users = await repo.Users.FilterAsync(x => x.Email == request.Email);

        return users.Any() ? Result.Failure("Email is already in use.") : Result.Success();
    }
}

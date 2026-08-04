using Mapster;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.UserInfo;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.UserInfo;

public sealed class UserInfoHandler(AuthRepository repo, IUserContext<AppUserId> userContext)
    : IWeavlyHandler<UserInfoCommand, Result>
{
    public async Task<Result> HandleAsync(UserInfoCommand _, CancellationToken ct = default)
    {
        try
        {
            var user = await repo.Users.FindAsync(x => x.Id == userContext.UserId, ct);

            return user is null ? Result.Failure("User not found") : Result.Success(user.Adapt<UserInfoResponse>());
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while retrieving user info: {ex.Message}");
        }
    }
}
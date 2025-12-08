using Mapster;
using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.UserInfo;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.UserInfo;

public sealed class UserInfoHandler(AuthDbContext dbContext, IUserContext<AppUserId> userContext)
    : IWeavlyHandler<UserInfoCommand, Result>
{
    public async Task<Result> HandleAsync(UserInfoCommand _, CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userContext.UserId, ct);

        return user is null ? Failure.Create("User not found") : Success.Create(user.Adapt<UserInfoResponse>());
    }
}

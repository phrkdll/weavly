using Weavly.Auth.Shared.Identifiers;

namespace Weavly.Auth.Shared.Features.UserInfo;

public sealed record UserInfoResponse(AppUserId? Id, string Email, DateTime? CreatedAt, DateTime? LastLoginAt)
{
    public static UserInfoResponse Empty => new(AppUserId.Empty, string.Empty, default, default);
}

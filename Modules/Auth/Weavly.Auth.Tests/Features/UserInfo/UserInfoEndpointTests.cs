using Weavly.Auth.Features.UserInfo;
using Weavly.Auth.Shared.Features.UserInfo;

namespace Weavly.Auth.Tests.Features.UserInfo;

public sealed class UserInfoEndpointTests : AuthEndpointTests<UserInfoEndpoint, UserInfoCommand>
{
    public UserInfoEndpointTests()
        : base(new UserInfoCommand()) { }
}

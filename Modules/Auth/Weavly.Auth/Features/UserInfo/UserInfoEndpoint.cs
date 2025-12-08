using Weavly.Auth.Shared.Features.UserInfo;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.UserInfo;

internal sealed class UserInfoEndpoint : GetEndpoint<UserInfoCommand, AuthModule>
{
    public UserInfoEndpoint(IMessageBus bus)
        : base("user", bus)
    {
        Authorize();
    }
}

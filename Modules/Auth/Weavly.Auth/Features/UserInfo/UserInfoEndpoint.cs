using Weavly.Auth.Shared.Features.UserInfo;

namespace Weavly.Auth.Features.UserInfo;

internal sealed class UserInfoEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("user/info");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await HandleDefaultAsync(new UserInfoCommand(), ct);
    }
}

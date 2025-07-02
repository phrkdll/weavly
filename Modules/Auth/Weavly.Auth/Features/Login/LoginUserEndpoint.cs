using Weavly.Auth.Shared.Features.Login;

namespace Weavly.Auth.Features.Login;

internal sealed class LoginUserEndpoint : Endpoint<LoginUserCommand>
{
    public override void Configure()
    {
        Post("user/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginUserCommand request, CancellationToken ct)
    {
        await HandleDefaultAsync(request, ct);
    }
}

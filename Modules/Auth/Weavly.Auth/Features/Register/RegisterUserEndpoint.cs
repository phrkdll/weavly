using Weavly.Auth.Shared.Features.Register;

namespace Weavly.Auth.Features.Register;

internal sealed class RegisterUserEndpoint : Endpoint<RegisterUserCommand>
{
    public override void Configure()
    {
        Post("user/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterUserCommand command, CancellationToken ct)
    {
        await HandleDefaultAsync(command, ct);
    }
}

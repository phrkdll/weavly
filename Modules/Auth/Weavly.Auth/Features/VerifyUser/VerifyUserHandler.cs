using Weavly.Auth.Enums;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.VerifyUser;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Shared.Features.SendMail;
using Wolverine;

namespace Weavly.Auth.Features.VerifyUser;

public sealed class VerifyUserHandler(AuthRepository repo, ITimeProvider timeProvider, IMessageBus bus)
    : IWeavlyHandler<VerifyUserCommand>
{
    public async Task<Result> HandleAsync(VerifyUserCommand command, CancellationToken ct = default)
    {
        using var session = await repo.StartTransactionAsync(ct);

        var user = await repo.Users.FindAsync(u => u.Tokens.Any(t => t.Value == command.Token), ct);

        var token = user?.Tokens.SingleOrDefault(t => t.Purpose == AppUserTokenPurpose.EmailValidation);

        if (user is null || token is null || timeProvider.UtcNow > token.ExpiresAt)
        {
            return Result.Failure("Email address verification failed.");
        }

        user.Tokens.Remove(token);
        user.Tokens.Add(AppUserToken.CreateLoginToken());

        await repo.Users.UpdateAsync(user, ct);

        await session.CommitTransactionAsync(ct);

        var model = new
        {
            BaseUrl = "http://localhost:5119",
            Token = user.Tokens.Single(x => x.Purpose == AppUserTokenPurpose.TokenLogin).Value,
        };

        await bus.InvokeAsync<Result>(SendMailCommand.Create<AuthModule>("VerifyUser", model, user.Email), ct);

        return Result.Success();
    }
}

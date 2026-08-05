using Weavly.Auth.Enums;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.Verification;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Shared.Features.SendMail;
using Wolverine;

namespace Weavly.Auth.Features.Verification;

public sealed class VerificationHandler(AuthRepository repo, ITimeProvider timeProvider, IMessageBus bus)
    : IWeavlyHandler<VerificationCommand>
{
    public async Task<Result> HandleAsync(VerificationCommand command, CancellationToken ct = default)
    {
        using var session = await repo.StartSessionAsync(ct);
        session.StartTransaction();

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
        await bus.InvokeAsync<Result>(VerificationSuccessMail(user), ct);

        return Result.Success(new VerificationResponse());
    }

    private static SendMailCommand VerificationSuccessMail(AppUser user)
    {
        var token = user.Tokens.Single(x => x.Purpose == AppUserTokenPurpose.TokenLogin).Value;

        var subject = "Weavly verification successful";

        var body = $"""
            <p>Hi!</p>
            <p>Your email address has been verified.</p>
            <p>You can login directly by following this link:
                <a href='http://localhost:5000/user/login?token={token}'>Login</a>
            </p>
            <p>You can also login via email and password later.</p>
            <p>Have a great time!</p>
            """;

        return new SendMailCommand(user.Email, subject, body);
    }
}

using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Enums;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.Verification;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Shared.Triggers;

namespace Weavly.Auth.Features.Verification;

public sealed class VerificationCommandHandler(AuthDbContext dbContext, ITimeProvider timeProvider)
    : ICommandHandler<VerificationCommand, Result>
{
    public async Task<Result> ExecuteAsync(VerificationCommand command, CancellationToken ct)
    {
        var user = await dbContext
            .Users.Include(x => x.Tokens)
            .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.Value == command.Token), ct);

        var token = user?.Tokens.FirstOrDefault(t => t.Purpose == AppUserTokenPurpose.EmailValidation);

        if (user is null || token is null || timeProvider.UtcNow > token.ExpiresAt)
        {
            return Failure.Create("Email address verification failed.");
        }

        user.Tokens.Remove(token);
        user.Tokens.Add(AppUserToken.CreateLoginToken());

        dbContext.Update(user);

        await dbContext.SaveChangesAsync(ct);
        await VerificationSuccessMail(user).ExecuteAsync(ct);

        return Success.Create(new VerificationResponse());
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

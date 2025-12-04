using System.Text;
using Google.Authenticator;
using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Contracts;
using Weavly.Auth.Enums;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.TwoFactorAuth.Verify;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.TwoFactorAuth.Verify;

public sealed class VerifyTwoFactorAuthCommandHandler(
    AuthDbContext dbContext,
    IJwtProvider jwtProvider,
    ITimeProvider timeProvider
) : ICommandHandler<VerifyTwoFactorAuthCommand, Result>
{
    public async Task<Result> ExecuteAsync(VerifyTwoFactorAuthCommand command, CancellationToken ct)
    {
        var user = await dbContext.Users.Include(x => x.Tokens).FirstOrDefaultAsync(x => x.Email == command.Email, ct);

        if (user is null)
        {
            return Failure.Create("User not found");
        }

        var authenticator = new TwoFactorAuthenticator();
        var token = user.GetUserToken(AppUserTokenPurpose.TwoFactorAuthentication);

        var pinValid = authenticator.ValidateTwoFactorPIN(
            Encoding.UTF8.GetBytes(token?.Value.ToString() ?? string.Empty),
            command.VerificationPin
        );

        if (!pinValid)
        {
            return Failure.Create("Invalid two factor authentication pin");
        }

        var utcNow = timeProvider.UtcNow;
        user.LastLoginAt = utcNow;
        user.LoginRequested = false;

        dbContext.Update(user);

        await dbContext.SaveChangesAsync(ct);

        var expiresAt = utcNow.AddMinutes(60);
        var jwt = await jwtProvider.GenerateTokenAsync(user, expiresAt);

        return Success.Create(new VerifyTwoFactorAuthResponse(jwt, expiresAt));
    }
}

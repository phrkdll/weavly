using System.Text;
using Google.Authenticator;
using Weavly.Auth.Enums;
using Weavly.Auth.Implementation.JsonWebToken;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.VerifyTwoFactorAuth;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.VerifyTwoFactorAuth;

public sealed class VerifyTwoFactorAuthHandler(
    AuthRepository repo,
    IJwtProvider jwtProvider,
    ITimeProvider timeProvider
) : IWeavlyHandler<VerifyTwoFactorAuthCommand>
{
    public async Task<Result> HandleAsync(VerifyTwoFactorAuthCommand command, CancellationToken ct = default)
    {
        using var session = await repo.StartSessionAsync(ct);
        session.StartTransaction();

        var user = await repo.Users.FindAsync(x => x.Email == command.Email, ct);

        if (user is null)
        {
            return Result.Failure("User not found");
        }

        var authenticator = new TwoFactorAuthenticator();
        var token = user.GetUserToken(AppUserTokenPurpose.TwoFactorAuthentication);

        var pinValid = authenticator.ValidateTwoFactorPIN(
            Encoding.UTF8.GetBytes(token?.Value.ToString() ?? string.Empty),
            command.VerificationPin
        );

        if (!pinValid)
        {
            return Result.Failure("Invalid two factor authentication pin");
        }

        var utcNow = timeProvider.UtcNow;
        user.LastLoginAt = utcNow;
        user.LoginRequested = false;

        await repo.Users.UpdateAsync(user, ct);

        await session.CommitTransactionAsync(ct);

        var expiresAt = utcNow.AddMinutes(60);
        var jwt = await jwtProvider.GenerateTokenAsync(user, expiresAt);

        return Result.Success(new VerifyTwoFactorAuthResponse(jwt, expiresAt));
    }
}

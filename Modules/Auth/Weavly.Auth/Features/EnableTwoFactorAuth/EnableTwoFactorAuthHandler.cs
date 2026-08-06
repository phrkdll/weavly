using System.Text;
using Google.Authenticator;
using Weavly.Auth.Enums;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.EnableTwoFactorAuth;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.EnableTwoFactorAuth;

public sealed class EnableTwoFactorAuthHandler(AuthRepository repo, IUserContext<AppUserId> userContext)
    : IWeavlyHandler<EnableTwoFactorAuthCommand>
{
    public async Task<Result> HandleAsync(EnableTwoFactorAuthCommand command, CancellationToken ct = default)
    {
        using var session = await repo.StartTransactionAsync(ct);

        var user = await repo.Users.FindAsync(x => x.Id == userContext.UserId, ct);

        if (user is null)
        {
            return Result.Failure("User not found");
        }

        if (user.Tokens.Any(x => x.Purpose == AppUserTokenPurpose.TwoFactorAuthentication))
        {
            return Result.Failure("2FA is already enabled");
        }

        var twoFactorAuthenticationToken = AppUserToken.CreateTwoFactorAuthenticationToken();
        var authenticator = new TwoFactorAuthenticator();

        var setupInfo = authenticator.GenerateSetupCode(
            "Weavly",
            user.Email,
            Encoding.UTF8.GetBytes(twoFactorAuthenticationToken.Value.ToString())
        );

        user.Tokens.Add(twoFactorAuthenticationToken);
        await repo.Users.UpdateAsync(user, ct);

        await session.CommitTransactionAsync(ct);

        return Result.Success(new EnableTwoFactorAuthResponse(setupInfo.ManualEntryKey, setupInfo.QrCodeSetupImageUrl));
    }
}

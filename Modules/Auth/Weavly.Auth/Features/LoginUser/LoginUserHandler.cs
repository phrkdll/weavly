using Microsoft.AspNetCore.Identity;
using Weavly.Auth.Enums;
using Weavly.Auth.Implementation.JsonWebToken;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.LoginUser;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.LoginUser;

public sealed class LoginUserHandler(
    AuthRepository repo,
    IPasswordHasher<AppUser> hasher,
    IJwtProvider jwtProvider,
    ITimeProvider timeProvider
) : IWeavlyHandler<LoginUserCommand>
{
    public async Task<Result> HandleAsync(LoginUserCommand command, CancellationToken ct = default)
    {
        using var session = await repo.StartSessionAsync(ct);
        session.StartTransaction();

        var user = await repo.Users.FindAsync(u => u.Email == command.Email, ct);

        if (user is null)
        {
            return Result.Failure("Email or password is incorrect.");
        }

        if (user.Tokens.Any(t => t.Purpose == AppUserTokenPurpose.EmailValidation))
        {
            return Result.Failure("Email verification pending.");
        }

        var passwordVerificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, command.Password);

        if (passwordVerificationResult is PasswordVerificationResult.Failed)
        {
            return Result.Failure("Email or password is incorrect.");
        }

        if (user.Tokens.Any(x => x.Purpose == AppUserTokenPurpose.TwoFactorAuthentication))
        {
            user.LoginRequested = true;
            await repo.Users.UpdateAsync(user, ct);

            await session.CommitTransactionAsync(ct);

            return Result.Success(LoginUserResponse.Empty);
        }

        var utcNow = timeProvider.UtcNow;
        user.LastLoginAt = utcNow;
        await repo.Users.UpdateAsync(user, ct);

        await session.CommitTransactionAsync(ct);

        var expiresAt = utcNow.AddMinutes(60);
        var jwt = await jwtProvider.GenerateTokenAsync(user, expiresAt);

        return Result.Success(new LoginUserResponse(jwt, expiresAt));
    }
}

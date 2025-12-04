using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Contracts;
using Weavly.Auth.Enums;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.Login;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.Login;

public sealed class LoginUserCommandHandler(
    AuthDbContext dbContext,
    PasswordHasher<AppUser> hasher,
    IJwtProvider jwtProvider,
    ITimeProvider timeProvider
) : ICommandHandler<LoginUserCommand, Result>
{
    public async Task<Result> ExecuteAsync(LoginUserCommand command, CancellationToken ct = default)
    {
        var user = dbContext.Users.Include(x => x.Tokens).SingleOrDefault(u => u.Email == command.Email);

        if (user is null)
        {
            return Failure.Create("Email or password is incorrect.");
        }

        if (user.Tokens.Any(t => t.Purpose == AppUserTokenPurpose.EmailValidation))
        {
            return Failure.Create("Email verification pending.");
        }

        var passwordVerificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, command.Password);

        if (passwordVerificationResult is PasswordVerificationResult.Failed)
        {
            return Failure.Create("Email or password is incorrect.");
        }

        if (user.Tokens.Any(x => x.Purpose == AppUserTokenPurpose.TwoFactorAuthentication))
        {
            user.LoginRequested = true;
            dbContext.Update(user);

            await dbContext.SaveChangesAsync(ct);

            return Success.Create(LoginUserResponse.Empty);
        }

        var utcNow = timeProvider.UtcNow;
        user.LastLoginAt = utcNow;

        dbContext.Update(user);

        await dbContext.SaveChangesAsync(ct);

        var expiresAt = utcNow.AddMinutes(60);
        var jwt = await jwtProvider.GenerateTokenAsync(user, expiresAt);

        return Success.Create(new LoginUserResponse(jwt, expiresAt));
    }
}

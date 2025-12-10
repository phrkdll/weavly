using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Contracts;
using Weavly.Auth.Enums;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.TokenLogin;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.TokenLogin;

public sealed class TokenLoginHandler(AuthDbContext dbContext, IJwtProvider jwtProvider)
    : IWeavlyHandler<TokenLoginCommand, Result>
{
    public async Task<Result> HandleAsync(TokenLoginCommand command, CancellationToken ct = default)
    {
        var user = await dbContext
            .Users.Include(x => x.Tokens)
            .SingleOrDefaultAsync(u => u.Tokens.Any(t => t.Value == command.Token), ct);

        var token = user?.Tokens.SingleOrDefault(t => t.Purpose == AppUserTokenPurpose.TwoFactorAuthentication);

        if (user is null || token is null)
        {
            return Failure.Create("Invalid login token.");
        }

        user.Tokens.Remove(token);

        dbContext.Update(user);

        await dbContext.SaveChangesAsync(ct);

        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var jwt = await jwtProvider.GenerateTokenAsync(user, expiresAt);

        return Success.Create(new TokenLoginResponse(jwt, expiresAt));
    }
}

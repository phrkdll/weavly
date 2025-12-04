using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Contracts;
using Weavly.Auth.Enums;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.TokenLogin;

namespace Weavly.Auth.Features.TokenLogin;

public sealed class TokenLoginCommandHandler(AuthDbContext dbContext, IJwtProvider jwtProvider)
    : ICommandHandler<TokenLoginCommand, Result>
{
    public async Task<Result> ExecuteAsync(TokenLoginCommand request, CancellationToken ct)
    {
        var user = dbContext
            .Users.Include(x => x.Tokens)
            .SingleOrDefault(u => u.Tokens.Any(t => t.Purpose == AppUserTokenPurpose.TokenLogin));

        var token = user?.Tokens.SingleOrDefault(t => t.Value == request.Token);

        if (user is null || token is null)
        {
            return Failure.Create("Invalid login token.");
        }

        user.Tokens?.Remove(token);

        dbContext.Update(user);

        await dbContext.SaveChangesAsync(ct);

        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var jwt = await jwtProvider.GenerateTokenAsync(user, expiresAt);

        return Success.Create(new TokenLoginResponse(jwt, expiresAt));
    }
}

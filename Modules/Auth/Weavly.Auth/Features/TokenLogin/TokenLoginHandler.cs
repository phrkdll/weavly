using Weavly.Auth.Enums;
using Weavly.Auth.Implementation.JsonWebToken;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.TokenLogin;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Features.TokenLogin;

public sealed class TokenLoginHandler(AuthRepository repo, IJwtProvider jwtProvider) : IWeavlyHandler<TokenLoginCommand>
{
    public async Task<Result> HandleAsync(TokenLoginCommand command, CancellationToken ct = default)
    {
        using var session = await repo.StartTransactionAsync(ct);

        var user = await repo.Users.FindAsync(u => u.Tokens.Any(t => t.Value == command.Token), ct);

        var token = user?.Tokens.SingleOrDefault(t => t.Purpose == AppUserTokenPurpose.TokenLogin);

        if (user is null || token is null)
        {
            return Result.Failure("Invalid login token.");
        }

        user.Tokens.Remove(token);

        await repo.Users.UpdateAsync(user, ct);

        await session.CommitTransactionAsync(ct);

        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var jwt = await jwtProvider.GenerateTokenAsync(user, expiresAt);

        return Result.Success(new TokenLoginResponse(jwt, expiresAt));
    }
}

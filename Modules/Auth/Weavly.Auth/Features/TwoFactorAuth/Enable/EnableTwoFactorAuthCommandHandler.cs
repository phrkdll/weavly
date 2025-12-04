using System.Security.Claims;
using System.Text;
using Google.Authenticator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Enums;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.TwoFactorAuth.Enable;
using Weavly.Auth.Shared.Identifiers;

namespace Weavly.Auth.Features.TwoFactorAuth.Enable;

public sealed class EnableTwoFactorAuthCommandHandler(AuthDbContext dbContext, IHttpContextAccessor contextAccessor)
    : ICommandHandler<EnableTwoFactorAuthCommand, Result>
{
    private readonly HttpContext _httpContext = contextAccessor.HttpContext ?? throw new NullReferenceException();

    public async Task<Result> ExecuteAsync(EnableTwoFactorAuthCommand command, CancellationToken ct)
    {
        var id =
            _httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        var user = await dbContext
            .Users.Include(x => x.Tokens)
            .FirstOrDefaultAsync(x => x.Id == AppUserId.Parse(id), ct);

        if (user is null)
        {
            return Failure.Create("User not found");
        }

        if (user.Tokens.Any(x => x.Purpose == AppUserTokenPurpose.TwoFactorAuthentication))
        {
            return Failure.Create("2FA is already enabled");
        }

        var twoFactorAuthenticationToken = AppUserToken.CreateTwoFactorAuthenticationToken();
        var authenticator = new TwoFactorAuthenticator();

        var setupInfo = authenticator.GenerateSetupCode(
            "Weavly",
            user.Email,
            Encoding.UTF8.GetBytes(twoFactorAuthenticationToken.Value.ToString())
        );

        user.Tokens.Add(twoFactorAuthenticationToken);
        dbContext.Update(user);

        await dbContext.SaveChangesAsync(ct);

        return Success.Create(new EnableTwoFactorAuthResponse(setupInfo.ManualEntryKey, setupInfo.QrCodeSetupImageUrl));
    }
}

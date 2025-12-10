using Weavly.Auth.Enums;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Models;

public sealed record AppUserToken(AppUserTokenPurpose Purpose, DateTime? ExpiresAt = null) : Entity<AppUserTokenId>
{
    public Guid Value { get; init; } = Guid.NewGuid();
}

public static class AppUserTokenFactory
{
    extension(AppUserToken)
    {
        public static AppUserToken CreateEmailValidationToken() =>
            new(AppUserTokenPurpose.EmailValidation, DateTime.UtcNow.AddHours(1));

        public static AppUserToken CreateLoginToken() =>
            new(AppUserTokenPurpose.TokenLogin, DateTime.UtcNow.AddMinutes(5));

        public static AppUserToken CreateTwoFactorAuthenticationToken() =>
            new(AppUserTokenPurpose.TwoFactorAuthentication);
    }
}

using Weavly.Auth.Enums;

namespace Weavly.Auth.Models;

public static class AppUserTokenFactory
{
    extension(AppUserToken)
    {
        public static AppUserToken CreateEmailValidationToken()
        {
            return new AppUserToken(AppUserTokenPurpose.EmailValidation, DateTime.UtcNow.AddHours(1));
        }

        public static AppUserToken CreateLoginToken()
        {
            return new AppUserToken(AppUserTokenPurpose.TokenLogin, DateTime.UtcNow.AddMinutes(5));
        }

        public static AppUserToken CreateTwoFactorAuthenticationToken()
        {
            return new AppUserToken(AppUserTokenPurpose.TwoFactorAuthentication);
        }
    }
}

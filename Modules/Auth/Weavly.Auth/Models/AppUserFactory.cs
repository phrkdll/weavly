using Weavly.Auth.Enums;

namespace Weavly.Auth.Models;

public static class AppUserFactory
{
    extension(AppUser user)
    {
        public bool IsEmailVerified => user.Tokens.Any(t => t.Purpose == AppUserTokenPurpose.EmailValidation);

        public AppUserToken? GetUserToken(AppUserTokenPurpose purpose)
        {
            return user.Tokens.FirstOrDefault(x => x.Purpose == purpose);
        }

        public static AppUser Create(string email, ICollection<AppUserToken> tokens)
        {
            return new AppUser { Email = email, Tokens = tokens };
        }

        public static AppUser Create(string email, string userName, AppRole initialRole)
        {
            return new AppUser
            {
                Email = email,
                UserName = userName,
                Roles = [initialRole.Id]
            };
        }
    }
}
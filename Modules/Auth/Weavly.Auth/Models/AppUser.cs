using System.ComponentModel.DataAnnotations;
using Weavly.Auth.Enums;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Models;

public sealed class AppUser : MetaEntity<AppUserId, AppUserId>
{
    private AppUser() { }

    [Required]
    [MaxLength(128)]
    public string Email { get; private init; } = string.Empty;

    [MaxLength(1024)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(32)]
    public string? UserName { get; init; }

    public ICollection<AppUserToken> Tokens { get; private init; } = [];

    public ICollection<AppRole> Roles { get; private init; } = [];

    public DateTime? LastLoginAt { get; set; }

    public bool LoginRequested { get; set; }

    public bool IsEmailVerified => Tokens.Any(t => t.Purpose == AppUserTokenPurpose.EmailValidation);

    public AppUserToken? GetUserToken(AppUserTokenPurpose purpose)
    {
        return Tokens.FirstOrDefault(x => x.Purpose == purpose);
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
            Roles = [initialRole],
        };
    }
}

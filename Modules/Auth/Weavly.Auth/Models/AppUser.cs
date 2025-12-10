using System.ComponentModel.DataAnnotations;
using Weavly.Auth.Enums;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Models;

public sealed record AppUser : MetaEntity<AppUserId, AppUserId>
{
    [Required]
    [MaxLength(128)]
    public string Email { get; init; } = string.Empty;

    [MaxLength(1024)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(32)]
    public string? UserName { get; init; }

    public ICollection<AppUserToken> Tokens { get; init; } = [];

    public ICollection<AppRole> Roles { get; init; } = [];

    public DateTime? LastLoginAt { get; set; }

    public bool LoginRequested { get; set; }

    public bool IsEmailVerified => Tokens.Any(t => t.Purpose == AppUserTokenPurpose.EmailValidation);

    public AppUserToken? GetUserToken(AppUserTokenPurpose purpose) => Tokens.FirstOrDefault(x => x.Purpose == purpose);

    public static AppUser Create(string email, ICollection<AppUserToken> tokens) =>
        new() { Email = email, Tokens = tokens };

    public static AppUser Create(string email, string userName, AppRole initialRole) =>
        new()
        {
            Email = email,
            UserName = userName,
            Roles = [initialRole],
        };
}

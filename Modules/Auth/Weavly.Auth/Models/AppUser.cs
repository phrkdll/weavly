using System.ComponentModel.DataAnnotations;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Models;

public sealed record AppUser : UserDocument<AppUserId, AppUserId>
{
    [Required]
    [MaxLength(128)]
    public string Email { get; init; } = string.Empty;

    [MaxLength(1024)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(32)]
    public string? UserName { get; init; }

    public ICollection<AppUserToken> Tokens { get; init; } = [];

    public ICollection<AppRoleId> Roles { get; init; } = [];

    public DateTime? LastLoginAt { get; set; }

    public bool LoginRequested { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Models;

public sealed class AppRole : Entity<AppRoleId>
{
    private AppRole() { }

    [Required]
    [MaxLength(24)]
    public string Name { get; private init; } = string.Empty;

    public ICollection<AppUser> Users { get; private init; } = [];

    public static AppRole Create(string name)
    {
        return new AppRole { Name = name };
    }
}

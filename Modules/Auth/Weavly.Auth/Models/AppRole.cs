using System.ComponentModel.DataAnnotations;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Models;

public sealed class AppRole : Entity<AppRoleId>
{
    [Required]
    [MaxLength(24)]
    public string Name { get; private init; }

    public ICollection<AppUser> Users { get; private init; } = [];

    public AppRole(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
    }

    public static AppRole Create(string name) => new(name);
}

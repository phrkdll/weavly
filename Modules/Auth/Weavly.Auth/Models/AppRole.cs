using System.ComponentModel.DataAnnotations;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Models;

public sealed record AppRole : Entity<AppRoleId>
{
    [Required]
    [MaxLength(24)]
    public string Name { get; init; }

    public ICollection<AppUser> Users { get; init; } = [];

    public AppRole(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
    }

    public static AppRole Create(string name) => new(name);
}

public static class AppRoleFactory
{
    extension(AppRole)
    {
        public static AppRole New(string name) => new(name);
    }
}

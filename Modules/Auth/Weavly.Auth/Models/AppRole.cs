using System.ComponentModel.DataAnnotations;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Models;

public sealed record AppRole : UserDocument<AppRoleId, AppUserId>
{
    public AppRole(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
    }

    [Required]
    [MinLength(2)]
    [MaxLength(24)]
    public string Name { get; init; }
}
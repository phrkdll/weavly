using System.ComponentModel.DataAnnotations;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Persistence.Models;

namespace Weavly.Configuration.Models;

public sealed class AppConfiguration : MetaEntity<ConfigurationId, AppUserId>
{
    [MaxLength(32)]
    public string Category { get; init; } = string.Empty;

    [MaxLength(64)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(64)]
    public string Module { get; init; } = string.Empty;

    [MaxLength(256)]
    public string? StringValue { get; init; }

    public double? DoubleValue { get; init; }

    public int? IntValue { get; init; }

    public bool? BoolValue { get; init; }
}

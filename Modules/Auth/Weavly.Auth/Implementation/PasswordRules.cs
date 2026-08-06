using Weavly.Auth.Seeding;
using Weavly.Configuration.Shared.Features.LoadConfiguration;

namespace Weavly.Auth.Implementation;

public sealed class PasswordRules
{
    public int MinimumLength { get; init; }
    public int MaximumLength { get; init; }
    public bool RequireDigit { get; init; }
    public bool RequireLowercase { get; init; }
    public bool RequireUppercase { get; init; }
    public bool RequireNonAlphanumeric { get; init; }

    public static PasswordRules FromConfigurationResponse(LoadConfigurationResponse config)
    {
        var minimumLength = config.GetInt(nameof(MinimumLength), ConfigCategory.PasswordRules) ?? 0;
        var maximumLength = config.GetInt(nameof(MaximumLength), ConfigCategory.PasswordRules) ?? 32;
        var requireDigit = config.GetBool(nameof(RequireDigit), ConfigCategory.PasswordRules) ?? false;
        var requireLowercase = config.GetBool(nameof(RequireLowercase), ConfigCategory.PasswordRules) ?? false;
        var requireUppercase = config.GetBool(nameof(RequireUppercase), ConfigCategory.PasswordRules) ?? false;
        var requireNonAlphanumeric =
            config.GetBool(nameof(RequireNonAlphanumeric), ConfigCategory.PasswordRules) ?? false;

        return new PasswordRules
        {
            MinimumLength = minimumLength,
            MaximumLength = maximumLength,
            RequireDigit = requireDigit,
            RequireLowercase = requireLowercase,
            RequireUppercase = requireUppercase,
            RequireNonAlphanumeric = requireNonAlphanumeric,
        };
    }
}

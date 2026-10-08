using Weavly.Auth.Implementation;
using Weavly.Configuration.Shared.Features.SeedConfiguration;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Seeding;

namespace Weavly.Auth.Seeding;

public sealed class ConfigSeed : WeavlySeed
{
    protected override IEnumerable<IWeavlyCommand> ProvideSeedingCommands() =>
        [
            SeedConfigurationCommand.Create<AuthModule>(
                "Secret",
                EncryptionKeyGenerator.GenerateAesKey(256),
                ConfigCategory.JsonWebToken
            ),
            SeedConfigurationCommand.Create<AuthModule>("Issuer", "https://weavly.api", ConfigCategory.JsonWebToken),
            SeedConfigurationCommand.Create<AuthModule>("Audience", "https://weavly.api", ConfigCategory.JsonWebToken),
            SeedConfigurationCommand.Create<AuthModule>("DisableEmailVerification", false, ConfigCategory.General),
            SeedConfigurationCommand.Create<AuthModule>("DisableUserRegistration", false, ConfigCategory.General),
            SeedConfigurationCommand.Create<AuthModule>("MinimumLength", 8, ConfigCategory.PasswordRules),
            SeedConfigurationCommand.Create<AuthModule>("MaximumLength", 32, ConfigCategory.PasswordRules),
            SeedConfigurationCommand.Create<AuthModule>("RequireUppercase", true, ConfigCategory.PasswordRules),
            SeedConfigurationCommand.Create<AuthModule>("RequireLowercase", true, ConfigCategory.PasswordRules),
            SeedConfigurationCommand.Create<AuthModule>("RequireDigit", true, ConfigCategory.PasswordRules),
            SeedConfigurationCommand.Create<AuthModule>("RequireNonAlphanumeric", true, ConfigCategory.PasswordRules),
        ];
}

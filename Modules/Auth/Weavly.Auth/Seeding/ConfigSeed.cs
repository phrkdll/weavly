using Weavly.Auth.Implementation;
using Weavly.Configuration.Shared.Features.CreateConfiguration;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Seeding;

namespace Weavly.Auth.Seeding;

public sealed class ConfigSeed : WeavlySeed
{
    protected override IEnumerable<IWeavlyCommand> ProvideSeedingCommands() =>
        [
            CreateConfigurationCommand.Create<AuthModule>(
                "Secret",
                EncryptionKeyGenerator.GenerateAesKey(256),
                ConfigCategory.JsonWebToken
            ),
            CreateConfigurationCommand.Create<AuthModule>("Issuer", "https://weavly.api", ConfigCategory.JsonWebToken),
            CreateConfigurationCommand.Create<AuthModule>(
                "Audience",
                "https://weavly.api",
                ConfigCategory.JsonWebToken
            ),
            CreateConfigurationCommand.Create<AuthModule>("DisableEmailVerification", false, ConfigCategory.General),
            CreateConfigurationCommand.Create<AuthModule>("DisableUserRegistration", false, ConfigCategory.General),
            CreateConfigurationCommand.Create<AuthModule>("MinimumLength", 8, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("MaximumLength", 32, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("RequireUppercase", true, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("RequireLowercase", true, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("RequireDigit", true, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("RequireNonAlphanumeric", true, ConfigCategory.PasswordRules),
        ];
}

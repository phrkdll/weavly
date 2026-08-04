using Weavly.Configuration.Shared.Features.LoadConfiguration;
using static Weavly.Auth.AuthModule;

namespace Weavly.Auth.Implementation;

public sealed class GeneralSettings
{
    public bool DisableEmailVerification { get; init; }
    public bool DisableUserRegistration { get; init; }
    public bool ForceTwoFactorAuth { get; init; }

    public static GeneralSettings FromConfigurationResponse(LoadConfigurationResponse config)
    {
        var disableEmailVerification =
            config.GetBool(nameof(DisableEmailVerification), ConfigCategory.GeneralSettings) ?? false;
        var disableUserRegistration =
            config.GetBool(nameof(DisableUserRegistration), ConfigCategory.GeneralSettings) ?? false;
        var forceTwoFactorAuth = config.GetBool(nameof(ForceTwoFactorAuth), ConfigCategory.GeneralSettings) ?? false;

        return new GeneralSettings
        {
            DisableEmailVerification = disableEmailVerification,
            DisableUserRegistration = disableUserRegistration,
            ForceTwoFactorAuth = forceTwoFactorAuth
        };
    }
}
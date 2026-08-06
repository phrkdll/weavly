using Weavly.Configuration.Shared.Features.LoadConfiguration;
using static Weavly.Auth.AuthModule;

namespace Weavly.Auth.Implementation;

public sealed record GeneralSettings(bool DisableEmailVerification, bool DisableUserRegistration)
{
    public static GeneralSettings FromConfigurationResponse(LoadConfigurationResponse config)
    {
        var disableEmailVerification =
            config.GetBool(nameof(DisableEmailVerification), ConfigCategory.GeneralSettings) ?? false;
        var disableUserRegistration =
            config.GetBool(nameof(DisableUserRegistration), ConfigCategory.GeneralSettings) ?? false;

        return new GeneralSettings(disableEmailVerification, disableUserRegistration);
    }
}

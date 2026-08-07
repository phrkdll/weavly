using Weavly.Auth.Seeding;
using Weavly.Configuration.Shared.Features.LoadConfiguration;

namespace Weavly.Auth.Implementation;

public sealed record GeneralSettings(bool DisableEmailVerification, bool DisableUserRegistration)
{
    public static GeneralSettings FromConfigurationResponse(LoadConfigurationResponse config)
    {
        var disableEmailVerification =
            config.GetBool(nameof(DisableEmailVerification), ConfigCategory.General) ?? false;
        var disableUserRegistration = config.GetBool(nameof(DisableUserRegistration), ConfigCategory.General) ?? false;

        return new GeneralSettings(disableEmailVerification, disableUserRegistration);
    }
}

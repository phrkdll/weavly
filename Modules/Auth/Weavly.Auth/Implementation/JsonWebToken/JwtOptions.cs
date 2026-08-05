using Weavly.Configuration.Shared.Features.LoadConfiguration;
using static Weavly.Auth.AuthModule;

namespace Weavly.Auth.Implementation.JsonWebToken;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string Secret { get; init; } = string.Empty;

    public static JwtOptions FromConfigurationResponse(LoadConfigurationResponse config)
    {
        var issuer = config.GetString(nameof(Issuer), ConfigCategory.JsonWebToken) ?? string.Empty;
        var audience = config.GetString(nameof(Audience), ConfigCategory.JsonWebToken) ?? string.Empty;
        var secret =
            config.GetString(nameof(Secret), ConfigCategory.JsonWebToken)
            ?? throw new ApplicationException("Missing secret key.");

        return new JwtOptions
        {
            Issuer = issuer,
            Audience = audience,
            Secret = secret,
        };
    }
}

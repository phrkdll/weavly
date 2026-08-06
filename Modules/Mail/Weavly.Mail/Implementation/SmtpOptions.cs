using Weavly.Configuration.Shared.Features.LoadConfiguration;
using static Weavly.Mail.MailModule;

namespace Weavly.Mail.Implementation;

public sealed class SmtpOptions
{
    public string SmtpHost { get; init; } = string.Empty;

    public int SmtpPort { get; init; }

    public bool EnableSsl { get; init; }

    public string SmtpUser { get; init; } = string.Empty;

    public string SmtpPassword { get; init; } = string.Empty;

    public string DefaultSender { get; init; } = string.Empty;

    public static SmtpOptions FromConfigurationResponse(LoadConfigurationResponse config)
    {
        var enableSsl = config.GetBool(nameof(EnableSsl), ConfigCategory.General) ?? true;
        var smtpHost = config.GetString(nameof(SmtpHost), ConfigCategory.General) ?? string.Empty;
        var smtpPort = config.GetInt(nameof(SmtpPort), ConfigCategory.General) ?? (enableSsl ? 143 : 25);
        var smtpUser = config.GetString(nameof(SmtpUser), ConfigCategory.General) ?? string.Empty;
        var smtpPassword = config.GetString(nameof(SmtpPassword), ConfigCategory.General) ?? string.Empty;
        var defaultSender = config.GetString(nameof(DefaultSender), ConfigCategory.General) ?? string.Empty;

        return new SmtpOptions
        {
            EnableSsl = enableSsl,
            SmtpHost = smtpHost,
            SmtpPort = smtpPort,
            SmtpUser = smtpUser,
            SmtpPassword = smtpPassword,
            DefaultSender = defaultSender,
        };
    }
}

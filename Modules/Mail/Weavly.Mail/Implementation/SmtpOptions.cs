using Weavly.Configuration.Shared.Features.LoadConfig;

namespace Weavly.Mail.Implementation;

public sealed class SmtpOptions
{
    public string SmtpHost { get; init; } = string.Empty;

    public int SmtpPort { get; init; }

    public bool EnableSsl { get; init; }

    public string SmtpUser { get; init; } = string.Empty;

    public string SmtpPassword { get; init; } = string.Empty;

    public string DefaultSender { get; init; } = string.Empty;

    public static SmtpOptions FromConfigurationResponse(LoadConfigurationResponse configurationResponse)
    {
        return new SmtpOptions
        {
            EnableSsl = configurationResponse[nameof(EnableSsl)].AsBool(),
            SmtpHost = configurationResponse[nameof(SmtpHost)].AsString(),
            SmtpPort = configurationResponse[nameof(SmtpPort)].AsInt(),
            SmtpUser = configurationResponse[nameof(SmtpUser)].AsString(),
            SmtpPassword = configurationResponse[nameof(SmtpPassword)].AsString(),
            DefaultSender = configurationResponse[nameof(DefaultSender)].AsString(),
        };
    }
}

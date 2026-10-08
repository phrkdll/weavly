using Weavly.Configuration.Shared.Features.SeedConfiguration;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Seeding;

namespace Weavly.Mail.Seeding;

public sealed class MailConfigurationSeed : WeavlySeed
{
    protected override IEnumerable<IWeavlyCommand> ProvideSeedingCommands() =>
        [
            SeedConfigurationCommand.Create<MailModule>(
                "DefaultSender",
                "no-reply@weavly.com",
                MailModule.ConfigCategory.General
            ),
            SeedConfigurationCommand.Create<MailModule>("SmtpHost", "localhost", MailModule.ConfigCategory.General),
            SeedConfigurationCommand.Create<MailModule>("SmtpPort", 1025, MailModule.ConfigCategory.General),
            SeedConfigurationCommand.Create<MailModule>("EnableSsl", false, MailModule.ConfigCategory.General),
            SeedConfigurationCommand.Create<MailModule>("SmtpUser", string.Empty, MailModule.ConfigCategory.General),
            SeedConfigurationCommand.Create<MailModule>(
                "SmtpPassword",
                string.Empty,
                MailModule.ConfigCategory.General
            ),
        ];
}

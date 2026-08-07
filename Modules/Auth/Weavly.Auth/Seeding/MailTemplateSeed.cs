using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Seeding;
using Weavly.Mail.Shared.Features.CreateMailTemplate;

namespace Weavly.Auth.Seeding;

internal sealed class MailTemplateSeed : WeavlySeed
{
    protected override IEnumerable<IWeavlyCommand> ProvideSeedingCommands() =>
        [
            CreateMailTemplateCommand.Create<AuthModule>(
                "RegisterUser",
                "Weavly verification mail",
                """
                <p>Hi!</p>
                <p>Please verify your email address by clicking the link below:</p>
                <p><a href='{{BaseUrl}}/user/verify?token={{Token}}'>Verify</a></p>
                """
            ),
        ];
}

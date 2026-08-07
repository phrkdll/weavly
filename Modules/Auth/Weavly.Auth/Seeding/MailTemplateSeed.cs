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
            CreateMailTemplateCommand.Create<AuthModule>(
                "VerifyUser",
                "Weavly verification successful",
                """
                <p>Hi!</p>
                <p>Your email address has been verified.</p>
                <p>You can login directly by following this link:
                    <a href='{{BaseUrl}}/user/login?token={{Token}}'>Login</a>
                </p>
                <p>You can also login via email and password later.</p>
                <p>Have a great time!</p>
                """
            ),
        ];
}

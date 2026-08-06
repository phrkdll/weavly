using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Models;
using Weavly.Mail.Shared.Identifiers;

namespace Weavly.Mail.Models;

public sealed record MailTemplate(string Module, string Name, string Template)
    : UserDocument<MailTemplateId, AppUserId>;
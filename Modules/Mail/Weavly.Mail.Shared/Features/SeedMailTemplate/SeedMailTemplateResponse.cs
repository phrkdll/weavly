using Weavly.Mail.Shared.Identifiers;

namespace Weavly.Mail.Shared.Features.SeedMailTemplate;

public sealed record SeedMailTemplateResponse(MailTemplateId MailTemplateId, bool Created);

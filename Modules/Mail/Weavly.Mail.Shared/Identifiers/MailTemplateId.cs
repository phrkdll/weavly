using Weavly.Core.Shared.Contracts;

namespace Weavly.Mail.Shared.Identifiers;

public record struct MailTemplateId(string Value) : IWeavlyId;
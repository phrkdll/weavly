using Weavly.Core.Shared.Contracts;

namespace Weavly.Mail.Shared.Features.SeedMailTemplate;

public sealed record SeedMailTemplateCommand(string Module, string Name, string Subject, string Text)
    : IWeavlyCommand
{
    public static SeedMailTemplateCommand Create<TModule>(string name, string subject, string text) =>
        new(typeof(TModule).Name, name, subject, text);
}

using Weavly.Core.Shared.Contracts;

namespace Weavly.Mail.Shared.Features.CreateMailTemplate;

public sealed record CreateMailTemplateCommand(string Module, string Name, string Subject, string Text) : IWeavlyCommand
{
    public static CreateMailTemplateCommand Create<T>(string name, string subject, string text) =>
        new(typeof(T).Name, name, subject, text);
}

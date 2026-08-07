using Weavly.Core.Shared.Contracts;

namespace Weavly.Mail.Shared.Features.CreateMailTemplate;

public sealed record CreateMailTemplateCommand(string Module, string Name, string Subject, string Template)
    : IWeavlyCommand
{
    public static CreateMailTemplateCommand Create<T>(string name, string subject, string template) =>
        new(typeof(T).Name, name, subject, template);
};
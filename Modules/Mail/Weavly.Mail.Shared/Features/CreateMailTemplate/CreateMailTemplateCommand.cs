using Weavly.Core.Shared.Contracts;

namespace Weavly.Mail.Shared.Features.CreateMailTemplate;

public sealed record CreateMailTemplateCommand(string Module, string Name, string Template) : IWeavlyCommand
{
    public static CreateMailTemplateCommand Create<T>(string name, string template) =>
        new(typeof(T).Name, name, template);
};
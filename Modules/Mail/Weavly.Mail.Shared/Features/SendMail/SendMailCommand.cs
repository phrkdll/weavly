using Weavly.Core.Shared.Contracts;

namespace Weavly.Mail.Shared.Features.SendMail;

public sealed record SendMailCommand(string Module, string Name, object Model, string To) : IWeavlyCommand
{
    public static SendMailCommand Create<T>(string name, object model, string to) =>
        new(typeof(T).Name, name, model, to);
};

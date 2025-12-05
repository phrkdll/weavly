using Weavly.Core.Shared.Contracts;

namespace Weavly.Mail.Shared.Features.SendMail;

public sealed record SendMailCommand(string To, string Subject, string Body) : IWeavlyCommand;

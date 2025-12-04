namespace Weavly.Mail.Shared.Triggers;

public sealed record SendMailCommand(string To, string Subject, string Body) : ICommand<Result>;

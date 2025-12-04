namespace Weavly.Auth.Shared.Features.TokenLogin;

public sealed record TokenLoginCommand(Guid Token) : ICommand<Result>;

namespace Weavly.Auth.Shared.Features.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<Result>;

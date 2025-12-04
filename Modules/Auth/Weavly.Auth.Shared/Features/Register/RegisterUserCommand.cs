namespace Weavly.Auth.Shared.Features.Register;

public sealed record RegisterUserCommand(string Email, string Password) : ICommand<Result>;

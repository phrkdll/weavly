using FastEndpoints;

namespace Weavly.Auth.Shared.Features.CreateAppUser;

public sealed record CreateAppUserCommand(string Email, string UserName, string InitialRole, string Password = "")
    : ICommand<Result>;

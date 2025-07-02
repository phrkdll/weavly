using FastEndpoints;

namespace Weavly.Auth.Shared.Features.CreateAppRole;

public sealed record CreateAppRoleCommand(string Name) : ICommand<Result>;

using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.CreateAppRole;

public sealed record CreateAppRoleCommand(string Name) : IWeavlyCommand;

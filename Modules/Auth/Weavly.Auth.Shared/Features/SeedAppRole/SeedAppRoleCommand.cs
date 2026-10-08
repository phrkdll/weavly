using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.SeedAppRole;

public sealed record SeedAppRoleCommand(string Name) : IWeavlyCommand;

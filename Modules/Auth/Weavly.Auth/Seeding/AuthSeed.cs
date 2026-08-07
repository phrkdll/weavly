using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Seeding;

namespace Weavly.Auth.Seeding;

public sealed class AuthSeed : WeavlySeed
{
    protected override IEnumerable<IWeavlyCommand> ProvideSeedingCommands() =>
        [
            new CreateAppUserCommand("system@weavly.local", "system", "System"),
            new CreateAppRoleCommand("Administrator"),
            new CreateAppRoleCommand("User"),
        ];
}

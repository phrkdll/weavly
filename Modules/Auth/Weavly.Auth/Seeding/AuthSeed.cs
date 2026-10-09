using Weavly.Auth.Shared.Features.SeedAppRole;
using Weavly.Auth.Shared.Features.SeedAppUser;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Seeding;

namespace Weavly.Auth.Seeding;

public sealed class AuthSeed : WeavlySeed
{
    protected override IEnumerable<IWeavlyCommand> ProvideSeedingCommands() =>
        [
            new SeedAppRoleCommand("System"),
            new SeedAppRoleCommand("Administrator"),
            new SeedAppRoleCommand("User"),
            new SeedAppUserCommand("system@weavly.local", "system", "System"),
        ];
}

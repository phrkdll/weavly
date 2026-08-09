using Weavly.Auth.Models;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Persistence;

namespace Weavly.Auth.Persistence;

public class AuthRepository(IWeavlyRepository<AuthModule> repository) : ModuleRepository<AuthModule>(repository)
{
    public IWeavlyCollection<AppUser, AppUserId> Users => Repository.GetCollectionFor<AppUser, AppUserId>();

    public IWeavlyCollection<AppRole, AppRoleId> Roles => Repository.GetCollectionFor<AppRole, AppRoleId>();
}

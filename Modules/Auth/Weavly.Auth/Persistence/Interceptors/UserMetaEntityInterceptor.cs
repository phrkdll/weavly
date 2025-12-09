using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Persistence.Interceptors;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Persistence.Interceptors;

public sealed class UserMetaEntityInterceptor(IUserContextFactory<AppUserId> userContextFactory)
    : CoreSaveChangesInterceptor<IUserMetaEntity<AppUserId>>
{
    private AppUserId userId;

    protected override void Prepare()
    {
        userId = userContextFactory.CreateUserContext().UserId;
    }

    protected override void HandleCreate(IUserMetaEntity<AppUserId> entity)
    {
        entity.CreatedBy = userId;

        HandleUpdate(entity);
    }

    protected override void HandleUpdate(IUserMetaEntity<AppUserId> entity)
    {
        entity.TouchedBy = userId;
    }
}

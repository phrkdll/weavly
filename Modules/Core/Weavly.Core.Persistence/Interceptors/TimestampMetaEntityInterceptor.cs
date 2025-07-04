using Weavly.Core.Persistence.Models;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Persistence.Interceptors;

public sealed class TimestampMetaEntityInterceptor(ITimeProvider timeProvider)
    : CoreSaveChangesInterceptor<ITimestampMetaEntity>
{
    private DateTime utcNow;

    protected override void Prepare()
    {
        utcNow = timeProvider.UtcNow;
    }

    protected override void HandleCreate(ITimestampMetaEntity entity)
    {
        entity.CreatedAt = utcNow;

        HandleUpdate(entity);
    }

    protected override void HandleUpdate(ITimestampMetaEntity entity)
    {
        entity.TouchedAt = utcNow;
    }
}

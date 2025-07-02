using Microsoft.EntityFrameworkCore.Diagnostics;
using Weavly.Core.Persistence.Models;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Persistence.Interceptors;

public sealed class MetaEntityInterceptor<TUserId>(
    ITimeProvider timeProvider,
    IUserContextFactory<TUserId> userContextFactory
) : SaveChangesInterceptor
    where TUserId : struct
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        var utcNow = timeProvider.UtcNow;
        var userId = userContextFactory.CreateUserContext().UserId;

        var dbContext = eventData.Context ?? throw new ArgumentException(nameof(eventData.Context));
        var entries = dbContext
            .ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is not IMetaEntity<TUserId> metaEntity)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    HandleCreate(metaEntity, utcNow, userId);
                    break;
                case EntityState.Modified:
                    HandleUpdate(metaEntity, utcNow, userId);
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void HandleCreate(IMetaEntity<TUserId> metaEntity, DateTime utcNow, TUserId userId)
    {
        metaEntity.CreatedAt = utcNow;
        metaEntity.CreatedBy = userId;

        HandleUpdate(metaEntity, utcNow, userId);
    }

    private void HandleUpdate(IMetaEntity<TUserId> metaEntity, DateTime utcNow, TUserId userId)
    {
        metaEntity.TouchedAt = utcNow;
        metaEntity.TouchedBy = userId;
    }
}

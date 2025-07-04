using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Weavly.Core.Persistence.Interceptors;

public abstract class CoreSaveChangesInterceptor<TEntity> : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        Prepare();

        var dbContext = eventData.Context ?? throw new ArgumentException(nameof(eventData.Context));
        var entries = dbContext
            .ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is not TEntity entity)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    HandleCreate(entity);
                    break;
                case EntityState.Modified:
                    HandleUpdate(entity);
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

    protected virtual void Prepare() { }

    protected abstract void HandleCreate(TEntity entity);

    protected abstract void HandleUpdate(TEntity entity);
}

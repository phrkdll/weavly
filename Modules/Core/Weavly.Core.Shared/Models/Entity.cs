namespace Weavly.Core.Shared.Models;

public abstract record Entity<TEntityId>
    where TEntityId : struct
{
    public TEntityId Id { get; init; }

    public bool IsDeleted { get; init; }

    public DateTime? DeletedAt { get; init; }
}

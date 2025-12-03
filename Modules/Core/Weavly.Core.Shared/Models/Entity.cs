namespace Weavly.Core.Shared.Models;

public abstract class Entity<TEntityId>
    where TEntityId : struct
{
    public TEntityId Id { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

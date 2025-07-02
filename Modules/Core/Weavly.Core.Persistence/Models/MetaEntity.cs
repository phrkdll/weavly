namespace Weavly.Core.Persistence.Models;

public class MetaEntity<TEntityId, TUserId> : Entity<TEntityId>, IMetaEntity<TUserId>
    where TEntityId : struct
    where TUserId : struct
{
    public DateTime? CreatedAt { get; set; }
    public TUserId? CreatedBy { get; set; }

    public DateTime? TouchedAt { get; set; }
    public TUserId? TouchedBy { get; set; }
}

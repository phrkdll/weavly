namespace Weavly.Core.Persistence.Models;

public interface IMetaEntity<TUserId>
    where TUserId : struct
{
    DateTime? CreatedAt { get; set; }
    TUserId? CreatedBy { get; set; }

    DateTime? TouchedAt { get; set; }
    TUserId? TouchedBy { get; set; }
}

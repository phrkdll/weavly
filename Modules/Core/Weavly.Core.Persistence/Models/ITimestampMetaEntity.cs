namespace Weavly.Core.Persistence.Models;

public interface ITimestampMetaEntity
{
    DateTime? CreatedAt { get; set; }

    DateTime? TouchedAt { get; set; }
}

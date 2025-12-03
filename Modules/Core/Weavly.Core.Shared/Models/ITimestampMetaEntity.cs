namespace Weavly.Core.Shared.Models;

public interface ITimestampMetaEntity
{
    DateTime? CreatedAt { get; set; }

    DateTime? TouchedAt { get; set; }
}

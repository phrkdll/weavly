namespace Weavly.Core.Persistence.Models;

public interface IUserMetaEntity<TUserId>
    where TUserId : struct
{
    TUserId? CreatedBy { get; set; }

    TUserId? TouchedBy { get; set; }
}

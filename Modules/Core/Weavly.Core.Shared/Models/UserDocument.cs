using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Shared.Models;

public abstract record UserDocument<TDocumentId, TUserId> : Document<TDocumentId>
    where TDocumentId : struct, IWeavlyId
    where TUserId : struct, IWeavlyId
{
    public TUserId? CreatedBy { get; init; }

    public TUserId? TouchedBy { get; init; }

    public TUserId? DeletedBy { get; init; }
}

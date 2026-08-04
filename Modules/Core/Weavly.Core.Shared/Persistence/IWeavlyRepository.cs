using MongoDB.Driver;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Models;

namespace Weavly.Core.Shared.Persistence;

public interface IWeavlyRepository<TModule>
    where TModule : IWeavlyModule
{
    IWeavlyCollection<T, TDocumentId> GetCollectionFor<T, TDocumentId>()
        where T : Document<TDocumentId>
        where TDocumentId : struct, IWeavlyId;

    Task<IClientSessionHandle> StartSessionAsync(CancellationToken ct = default);
}
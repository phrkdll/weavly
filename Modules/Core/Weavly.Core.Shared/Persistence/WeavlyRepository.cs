using MongoDB.Driver;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Models;

namespace Weavly.Core.Shared.Persistence;

public class WeavlyRepository<TModule>(IMongoClient client, ITimeProvider timeProvider) : IWeavlyRepository<TModule>
    where TModule : IWeavlyModule
{
    public IWeavlyCollection<T, TDocumentId> GetCollectionFor<T, TDocumentId>()
        where T : Document<TDocumentId>
        where TDocumentId : struct, IWeavlyId
    {
        return new WeavlyCollection<T, TDocumentId>(
            client.GetDatabase(typeof(TModule).Name).GetCollection<T>(typeof(T).Name),
            timeProvider
        );
    }

    public Task<IClientSessionHandle> StartSessionAsync(CancellationToken ct = default)
    {
        return client.StartSessionAsync(
            new ClientSessionOptions
            {
                CausalConsistency = true,
                DefaultTransactionOptions = new TransactionOptions(
                    ReadConcern.Available,
                    writeConcern: WriteConcern.Acknowledged
                )
            },
            ct
        );
    }
}
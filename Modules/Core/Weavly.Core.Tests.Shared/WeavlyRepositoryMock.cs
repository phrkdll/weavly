using MongoDB.Driver;
using NSubstitute;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Models;
using Weavly.Core.Shared.Persistence;

namespace Weavly.Core.Tests.Shared;

public class WeavlyRepositoryMock<TModule>(ITimeProvider timeProvider) : IWeavlyRepository<TModule>
    where TModule : IWeavlyModule
{
    private readonly Dictionary<Type, object> collections = [];

    public IWeavlyCollection<T, TDocumentId> GetCollectionFor<T, TDocumentId>()
        where T : Document<TDocumentId>
        where TDocumentId : struct, IWeavlyId
    {
        if (!this.collections.ContainsKey(typeof(T)))
        {
            this.collections.Add(typeof(T), new WeavlyCollectionMock<T, TDocumentId>(timeProvider));
        }

        return (IWeavlyCollection<T, TDocumentId>)this.collections[typeof(T)];
    }

    public Task<IClientSessionHandle> StartSessionAsync(CancellationToken ct = default)
    {
        try
        {
            return Task.FromResult(Substitute.For<IClientSessionHandle>());
        }
        catch (Exception exception)
        {
            return Task.FromException<IClientSessionHandle>(exception);
        }
    }
}
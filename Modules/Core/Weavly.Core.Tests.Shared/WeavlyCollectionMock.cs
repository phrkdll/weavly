using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Models;
using Weavly.Core.Shared.Persistence;

namespace Weavly.Core.Tests.Shared;

public sealed class WeavlyCollectionMock<T, TDocumentId>(ITimeProvider timeProvider) : IWeavlyCollection<T, TDocumentId>
    where T : Document<TDocumentId>
    where TDocumentId : struct, IWeavlyId
{
    private readonly HashSet<T> collection = [];

    public Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
    {
        var documents = this.collection.Where(filter.Compile());

        return Task.FromResult(documents);
    }

    public Task<T?> FindAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
    {
        var documents = this.collection.SingleOrDefault(filter.Compile());

        return Task.FromResult(documents);
    }

    public Task InsertAsync(T document, CancellationToken ct = default)
    {
        document.Id = (TDocumentId)Activator.CreateInstance(typeof(TDocumentId), ObjectId.GenerateNewId().ToString())!;
        document.CreatedAt = document.TouchedAt = timeProvider.UtcNow;

        this.collection.Add(document);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(T document, CancellationToken ct = default)
    {
        this.collection.RemoveWhere(x => x.Id.Equals(document.Id));

        document.TouchedAt = timeProvider.UtcNow;

        this.collection.Add(document);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(TDocumentId id, CancellationToken ct = default)
    {
        var document = this.collection.SingleOrDefault(x => x.Id.Equals(id));

        document?.DeletedAt = timeProvider.UtcNow;

        return Task.CompletedTask;
    }

    public IQueryable<T> Query(AggregateOptions? aggregateOptions = null) => this.collection.AsQueryable();
}
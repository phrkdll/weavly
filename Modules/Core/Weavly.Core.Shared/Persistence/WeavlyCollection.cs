using System.Linq.Expressions;
using MongoDB.Driver;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Models;

namespace Weavly.Core.Shared.Persistence;

public sealed class WeavlyCollection<T, TDocumentId>(IMongoCollection<T> collection, ITimeProvider timeProvider)
    : IWeavlyCollection<T, TDocumentId>
    where T : Document<TDocumentId>
    where TDocumentId : struct, IWeavlyId
{
    public async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
    {
        var expressionFilter = new ExpressionFilterDefinition<T>(filter);

        var documents = await collection.FindAsync(expressionFilter, null, ct);

        return await documents.ToListAsync(ct);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default)
    {
        var expressionFilter = new ExpressionFilterDefinition<T>(filter);

        var documents = await collection.FindAsync(expressionFilter, null, ct);

        return await documents.SingleOrDefaultAsync(ct);
    }

    public Task InsertAsync(T document, CancellationToken ct = default)
    {
        document.CreatedAt = document.TouchedAt = timeProvider.UtcNow;

        return collection.InsertOneAsync(document, null, ct);
    }

    public Task UpdateAsync(T document, CancellationToken ct = default)
    {
        var expressionFilter = new ExpressionFilterDefinition<T>(x => x.Id.Equals(document.Id));

        document.TouchedAt = timeProvider.UtcNow;

        return collection.ReplaceOneAsync(expressionFilter, document, null as ReplaceOptions, ct);
    }

    public async Task DeleteAsync(TDocumentId id, CancellationToken ct = default)
    {
        var expressionFilter = new ExpressionFilterDefinition<T>(x => x.Id.Equals(id));

        var documents = await collection.FindAsync(expressionFilter, null, ct);
        var document = await documents.SingleOrDefaultAsync(ct);

        document.DeletedAt = timeProvider.UtcNow;

        _ = await collection.ReplaceOneAsync(expressionFilter, document, null as ReplaceOptions, ct);
    }

    public IQueryable<T> Query(AggregateOptions? options = null)
    {
        return collection.AsQueryable(options);
    }
}

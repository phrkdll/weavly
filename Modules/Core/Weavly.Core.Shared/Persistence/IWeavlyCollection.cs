using System.Linq.Expressions;
using MongoDB.Driver;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Models;

namespace Weavly.Core.Shared.Persistence;

public interface IWeavlyCollection<T, in TDocumentId>
    where T : Document<TDocumentId>
    where TDocumentId : struct, IWeavlyId
{
    Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);

    Task<T?> FindAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);

    Task InsertAsync(T document, CancellationToken ct = default);

    Task UpdateAsync(T document, CancellationToken ct = default);

    Task DeleteAsync(TDocumentId id, CancellationToken ct = default);

    IQueryable<T> Query(AggregateOptions? aggregateOptions = null);
}

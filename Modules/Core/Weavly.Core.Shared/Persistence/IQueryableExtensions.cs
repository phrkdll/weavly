using MongoDB.Driver.Linq;

namespace Weavly.Core.Shared.Persistence;

public static class IQueryableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query, CancellationToken ct = default)
    {
        try
        {
            return await MongoQueryable.ToListAsync(query, ct);
        }
        catch (ArgumentException ex) when (ex.ParamName == "source")
        {
            return [.. query];
        }
    }
}

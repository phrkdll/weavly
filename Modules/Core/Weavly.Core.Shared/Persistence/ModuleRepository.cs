using MongoDB.Driver;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Shared.Persistence;

public abstract class ModuleRepository<TModule>(IWeavlyRepository<TModule> repository)
    where TModule : IWeavlyModule
{
    protected readonly IWeavlyRepository<TModule> Repository = repository;

    /// <summary>
    /// Starts a session via <see cref="IMongoClient"/>
    /// </summary>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    public Task<IClientSessionHandle> StartSessionAsync(CancellationToken ct = default)
    {
        return this.Repository.StartSessionAsync(ct);
    }

    /// <summary>
    /// Starts a session via <see cref="IMongoClient"/> with an active transaction
    /// </summary>
    /// <param name="ct"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    public async Task<IClientSessionHandle> StartTransactionAsync(CancellationToken ct = default)
    {
        var session = await this.Repository.StartSessionAsync(ct);
        session.StartTransaction();

        return session;
    }
}

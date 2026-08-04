using MongoDB.Driver;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Shared.Persistence;

public abstract class ModuleRepository<TModule>(IWeavlyRepository<TModule> repository)
    where TModule : IWeavlyModule
{
    protected readonly IWeavlyRepository<TModule> Repository = repository;

    public Task<IClientSessionHandle> StartSessionAsync(CancellationToken ct = default)
    {
        return this.Repository.StartSessionAsync(ct);
    }
}
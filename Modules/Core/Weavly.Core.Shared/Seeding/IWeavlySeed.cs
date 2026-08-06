using Wolverine;

namespace Weavly.Core.Shared.Seeding;

public interface IWeavlySeed
{
    public Task<Result> SeedAsync(IMessageBus bus, CancellationToken ct = default);
}
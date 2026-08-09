using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Seeding;

public abstract class WeavlySeed : IWeavlySeed
{
    public async Task<Result> SeedAsync(IMessageBus bus, CancellationToken ct = default)
    {
        try
        {
            List<Result> results = [];

            foreach (var command in ProvideSeedingCommands())
            {
                results.Add(await bus.InvokeAsync<Result>(command, ct));
            }

            return results.All(x => x.Success)
                ? Result.Success()
                : Result.Failure($"Seeding of {GetType().Name} failed.");
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }

    protected abstract IEnumerable<IWeavlyCommand> ProvideSeedingCommands();
}

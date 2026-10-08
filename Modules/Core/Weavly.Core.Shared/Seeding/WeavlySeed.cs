using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Seeding;

public abstract class WeavlySeed : IWeavlySeed
{
    public async Task<Result> SeedAsync(IMessageBus bus, CancellationToken ct = default)
    {
        try
        {
            foreach (var command in ProvideSeedingCommands())
            {
                var result = await bus.InvokeAsync<Result>(command, ct);
                if (!result.Success)
                {
                    return Result.Failure(
                        $"Seeding of {GetType().Name} failed: {result.Message}",
                        result as Failure
                    );
                }
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }

    protected abstract IEnumerable<IWeavlyCommand> ProvideSeedingCommands();
}

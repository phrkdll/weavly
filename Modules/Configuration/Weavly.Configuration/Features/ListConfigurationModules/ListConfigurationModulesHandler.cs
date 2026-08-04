using MongoDB.Driver.Linq;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared.Features.ListConfigurationModules;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.ListConfigurationModules;

public class ListConfigurationModulesHandler(ConfigurationRepository repo)
    : IWeavlyHandler<ListConfigurationModulesCommand, Result>
{
    public async Task<Result> HandleAsync(ListConfigurationModulesCommand command,
        CancellationToken ct = default)
    {
        try
        {
            var modules = await repo.Configurations.Query().Select(x => x.Module).Distinct().ToListAsync(ct);

            return Result.Success(new ListConfigurationModulesResponse(modules));
        }
        catch (Exception e)
        {
            return Result.Failure(e, e.Message);
        }
    }
}
using Mapster;
using Microsoft.Extensions.Logging;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.LoadConfiguration;

public sealed class LoadConfigurationHandler(ConfigurationRepository repo) : IWeavlyHandler<LoadConfigurationCommand>
{
    public async Task<Result> HandleAsync(LoadConfigurationCommand command, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            var configurations = await repo
                .Configurations.FilterAsync(x => x.Module == command.Module, ct)
                .ContinueWith(x => x.Result.ToArray(), ct);

            if (configurations.Length == 0)
            {
                return Result.Failure("Could not find configuration");
            }

            var converted = configurations.Select(x => x.Adapt<ConfigurationItem>());

            return Result.Success(new LoadConfigurationResponse(command.Module, converted));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }
}

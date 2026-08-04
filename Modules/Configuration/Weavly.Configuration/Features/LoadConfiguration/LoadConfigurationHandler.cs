using Mapster;
using Microsoft.Extensions.Logging;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.LoadConfiguration;

public sealed class LoadConfigurationHandler(ConfigurationRepository repo, ILogger<LoadConfigurationHandler> logger)
    : IWeavlyHandler<LoadConfigurationCommand, Result>
{
    public async Task<Result> HandleAsync(LoadConfigurationCommand command, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            logger.LogInformation("Received {MessageType} message", nameof(LoadConfigurationCommand));

            var configurations = await repo.Configurations.FilterAsync(x => x.Module == command.Module, ct);

            if (!configurations.Any())
            {
                return Result.Failure("Could not find configuration");
            }

            var converted = configurations.Select(x => x.Adapt<ConfigurationResponse>());

            return Result.Success(new LoadConfigurationResponse(command.Module, converted));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }
}
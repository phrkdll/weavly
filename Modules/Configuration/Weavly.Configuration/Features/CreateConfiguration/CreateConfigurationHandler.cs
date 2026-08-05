using Mapster;
using Microsoft.Extensions.Logging;
using Weavly.Configuration.Models;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared.Features.CreateConfiguration;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.CreateConfiguration;

public sealed class CreateConfigurationHandler(ConfigurationRepository repo, ILogger<CreateConfigurationHandler> logger)
    : IWeavlyHandler<CreateConfigurationCommand>
{
    public async Task<Result> HandleAsync(CreateConfigurationCommand command, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            using var session = await repo.StartSessionAsync(ct);
            session.StartTransaction();

            logger.LogDebug("Received {Command}", command);

            if (await IsModuleConfigurationNameAvailable(command) is Failure notAvailableFailure)
            {
                logger.LogDebug("Configuration entry {Name} for {Module} already exists", command.Name, command.Module);
                return notAvailableFailure;
            }

            logger.LogDebug("Configuration entry {Name} for {Module} created", command.Name, command.Module);
            var configuration = command.Adapt<AppConfiguration>();
            await repo.Configurations.InsertAsync(configuration, ct);

            await session.CommitTransactionAsync(ct);

            return Result.Success(configuration.Id);
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }

    private async Task<Result> IsModuleConfigurationNameAvailable(CreateConfigurationCommand command)
    {
        var configuration = await repo.Configurations.FindAsync(x =>
            x.Module == command.Module && x.Name == command.Name
        );

        return configuration is null ? Result.Success() : Result.Failure("Configuration can't be registered");
    }
}

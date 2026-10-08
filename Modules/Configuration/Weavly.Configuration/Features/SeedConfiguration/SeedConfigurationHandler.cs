using Microsoft.Extensions.Logging;
using Weavly.Configuration.Models;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared.Features.SeedConfiguration;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.SeedConfiguration;

public sealed class SeedConfigurationHandler(
    ConfigurationRepository repo,
    ILogger<SeedConfigurationHandler> logger
) : IWeavlyHandler<SeedConfigurationCommand>
{
    public async Task<Result> HandleAsync(SeedConfigurationCommand command, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            using var session = await repo.StartTransactionAsync(ct);

            var existing = await repo.Configurations.FindAsync(
                x => x.Module == command.Module && x.Name == command.Name,
                ct
            );
            if (existing is not null)
            {
                logger.LogDebug(
                    "Seeded configuration entry {Module} -> {Name} already exists",
                    command.Module,
                    command.Name
                );
                return Result.Success(new SeedConfigurationResponse(existing.Id, false));
            }

            var configuration = new AppConfiguration
            {
                Module = command.Module,
                Name = command.Name,
                Category = command.Category,
                StringValue = command.StringValue,
                BoolValue = command.BoolValue,
                IntValue = command.IntValue,
                DoubleValue = command.DoubleValue,
                ValueType = command.ValueType,
            };
            await repo.Configurations.InsertAsync(configuration, ct);
            await session.CommitTransactionAsync(ct);

            logger.LogDebug("Seeded configuration entry {Module} -> {Name}", command.Module, command.Name);
            return Result.Success(new SeedConfigurationResponse(configuration.Id, true));
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }
}

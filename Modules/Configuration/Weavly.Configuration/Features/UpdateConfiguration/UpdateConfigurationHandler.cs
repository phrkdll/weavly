using Microsoft.Extensions.Logging;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared.Features.UpdateConfiguration;
using Weavly.Configuration.Shared.Models;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.UpdateConfiguration;

public sealed class UpdateConfigurationHandler(
    ConfigurationRepository repo,
    IValidator<UpdateConfigurationCommand> validator,
    ILogger<UpdateConfigurationHandler> logger
) : IWeavlyHandler<UpdateConfigurationCommand>
{
    public async Task<Result> HandleAsync(UpdateConfigurationCommand command, CancellationToken ct = default)
    {
        if (await validator.ValidateAsync(command, ct) is ValidationFailure validationFailure)
        {
            return validationFailure;
        }

        using var session = await repo.StartTransactionAsync(ct);

        var configuration = await repo.Configurations.FindAsync(x => x.Id == command.Id, ct);
        if (configuration is null)
        {
            logger.LogDebug("Configuration not found: {Id}", command.Id);

            return Result.Failure("Configuration not found");
        }

        var hasValueTypeMismatch = configuration.ValueType switch
        {
            ConfigurationValueType.String
                when command is { StringValue: not null, BoolValue: null, DoubleValue: null, IntValue: null } => false,
            ConfigurationValueType.Bool
                when command is { StringValue: null, BoolValue: not null, DoubleValue: null, IntValue: null } => false,
            ConfigurationValueType.Int
                when command is { StringValue: null, BoolValue: null, DoubleValue: null, IntValue: not null } => false,
            ConfigurationValueType.Double
                when command is { StringValue: null, BoolValue: null, DoubleValue: not null, IntValue: null } => false,
            _ => true,
        };

        if (hasValueTypeMismatch)
        {
            logger.LogError(
                "Configuration value type ({OriginalType}) may not be changed. {Command}",
                configuration.ValueType,
                command
            );

            return Result.Failure("Configuration value type may not be changed");
        }

        var updatedConfiguration = configuration with
        {
            StringValue = command.StringValue,
            IntValue = command.IntValue,
            BoolValue = command.BoolValue,
            DoubleValue = command.DoubleValue,
        };

        await repo.Configurations.UpdateAsync(updatedConfiguration, ct);

        logger.LogDebug("Configuration updated: {Id}", command.Id);

        await session.CommitTransactionAsync(ct);

        return Result.Success();
    }
}

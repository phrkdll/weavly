using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Weavly.Configuration.Shared.Features.UpdateConfiguration;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.UpdateConfiguration;

public sealed class UpdateConfigurationCommandValidator(ILogger<UpdateConfigurationCommandValidator> logger)
    : IValidator<UpdateConfigurationCommand>
{
    public async Task<Result> ValidateAsync(UpdateConfigurationCommand command, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        IList<ValidationResult?> results = [ValidateValueScalarity(command)];

        return results.Any(x => x != null) ? Result.ValidationFailure(results.Where(x => x != null)) : Result.Success();
    }

    private ValidationResult? ValidateValueScalarity(UpdateConfigurationCommand command)
    {
        object?[] values = [command.BoolValue, command.DoubleValue, command.StringValue, command.IntValue];
        object?[] actualValues = [.. values.Where(x => x is not null)];

        if (actualValues.Length <= 1)
        {
            return ValidationResult.Success;
        }

        logger.LogError("Multiple configuration values found: {Values}", actualValues);

        return new ValidationResult("Only one configuration value/type is allowed per entry");
    }
}

using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Weavly.Configuration.Models;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared.Features.CreateConfiguration;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.CreateConfiguration;

public sealed class CreateConfigurationCommandHandler(
    ConfigurationDbContext dbContext,
    ILogger<CreateConfigurationCommandHandler> logger
) : IWeavlyCommandHandler<CreateConfigurationCommand, Result>
{
    public async Task<Result> HandleAsync(CreateConfigurationCommand command, CancellationToken ct)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            logger.LogInformation("Received {MessageType} message", nameof(CreateConfigurationCommand));

            if (await ConfigurationCanNotBeRegisteredAsync(command))
            {
                return Failure.Create("Configuration can't be registered");
            }

            var configuration = command.Adapt<AppConfiguration>();
            dbContext.Configurations.Add(configuration);

            await dbContext.SaveChangesAsync(ct);

            return Success.Create(configuration.Id);
        }
        catch (Exception e)
        {
            return Failure.Create(e);
        }
    }

    private async Task<bool> ConfigurationCanNotBeRegisteredAsync(CreateConfigurationCommand command) =>
        await dbContext.Configurations.AnyAsync(x => x.Module == command.Module && x.Name == command.Name);
}

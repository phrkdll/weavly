using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared;
using Weavly.Configuration.Shared.Features.LoadConfig;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Features.LoadConfiguration;

public sealed class LoadConfigurationCommandHandler(
    ConfigurationDbContext dbContext,
    ILogger<LoadConfigurationCommandHandler> logger
) : IWeavlyCommandHandler<LoadConfigurationCommand, Result>
{
    public async Task<Result> HandleAsync(LoadConfigurationCommand command, CancellationToken ct)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(command);

            logger.LogInformation("Received {MessageType} message", nameof(LoadConfigurationCommand));

            var queryResult = await dbContext
                .Configurations.Where(x => x.Module == command.Module)
                .ToListAsync(cancellationToken: ct);

            if (queryResult.Count == 0)
            {
                return Failure.Create("Could not find configuration");
            }

            var converted = queryResult.Select(x => x.Adapt<ConfigurationResponse>());

            return Success.Create(new LoadConfigurationResponse(command.Module, converted));
        }
        catch (Exception e)
        {
            return Failure.Create(e);
        }
    }
}

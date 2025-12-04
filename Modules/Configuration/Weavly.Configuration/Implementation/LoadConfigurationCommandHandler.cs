using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared;
using Weavly.Configuration.Shared.Features.LoadConfig;

namespace Weavly.Configuration.Implementation;

public sealed class LoadConfigurationCommandHandler(
    ConfigurationDbContext dbContext,
    ILogger<LoadConfigurationCommandHandler> logger
) : ICommandHandler<LoadConfigurationCommand, Result>
{
    public async Task<Result> ExecuteAsync(LoadConfigurationCommand request, CancellationToken ct)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            logger.LogInformation("Received {MessageType} message", nameof(LoadConfigurationCommand));

            var queryResult = await dbContext
                .Configurations.Where(x => x.Module == request.Module)
                .ToListAsync(cancellationToken: ct);

            if (queryResult.Count == 0)
            {
                return Failure.Create("Could not find configuration");
            }

            var converted = queryResult.Select(x => x.Adapt<ConfigurationResponse>());

            return Success.Create(new LoadConfigurationResponse(request.Module, converted));
        }
        catch (Exception e)
        {
            return Failure.Create(e);
        }
    }
}

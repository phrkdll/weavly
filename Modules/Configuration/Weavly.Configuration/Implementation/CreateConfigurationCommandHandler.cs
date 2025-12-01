using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Weavly.Configuration.Models;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared;

namespace Weavly.Configuration.Implementation;

public sealed class CreateConfigurationCommandHandler(
    ConfigurationDbContext dbContext,
    ILogger<CreateConfigurationCommandHandler> logger
) : ICommandHandler<CreateConfigurationCommand, Result>
{
    public async Task<Result> ExecuteAsync(CreateConfigurationCommand request, CancellationToken ct)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);

            logger.LogInformation("Received {MessageType} message", nameof(CreateConfigurationCommand));

            if (await ConfigurationCanNotBeRegisteredAsync(request))
            {
                return Failure.Create("Configuration can't be registered");
            }

            var configuration = request.Adapt<AppConfiguration>();
            dbContext.Configurations.Add(configuration);

            await dbContext.SaveChangesAsync(ct);

            return Success.Create(configuration.Id);
        }
        catch (Exception e)
        {
            return Failure.Create(e);
        }
    }

    private async Task<bool> ConfigurationCanNotBeRegisteredAsync(CreateConfigurationCommand request) =>
        await dbContext.Configurations.AnyAsync(x => x.Module == request.Module && x.Name == request.Name);
}

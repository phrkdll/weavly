using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Configuration.Shared.Extensions;

public static class IMessageBusExtensions
{
    extension(IMessageBus bus)
    {
        public async Task<LoadConfigurationResponse> LoadConfigurationAsync<T>(CancellationToken ct = default)
            where T : IWeavlyModule
        {
            var command = LoadConfigurationCommand.Create<T>();
            var response = await bus.InvokeAsync<Result>(command, ct);

            return response is not Success<LoadConfigurationResponse> config
                ? throw new InvalidOperationException(response.Message)
                : config.Data;
        }
    }
}
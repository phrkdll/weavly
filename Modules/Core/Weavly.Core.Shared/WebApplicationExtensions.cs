using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        /// <summary>
        ///     Find and register module specific minimal API endpoints
        /// </summary>
        public int MapEndpoints(IWeavlyModule moduleType)
        {
            using var scope = app.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            var instances = moduleType
                .GetType()
                .Assembly.DefinedTypes.Where(x => typeof(IWeavlyEndpoint).IsAssignableFrom(x))
                .Select(t => Activator.CreateInstance(t, bus) as IWeavlyEndpoint);

            foreach (var instance in instances)
            {
                instance?.MapEndpoint(app);
            }

            return instances.Count();
        }
    }
}

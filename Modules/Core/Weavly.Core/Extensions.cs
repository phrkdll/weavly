using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core;

public static class Extensions
{
    private static WeavlyApplicationBuilder? _weavlyApplicationBuilder;

    /// <summary>
    ///     Add Weavly core components to the application
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IHostApplicationBuilder" />
    /// </param>
    /// <returns>
    ///     <see cref="IWeavlyApplicationBuilder" />
    /// </returns>
    public static IWeavlyApplicationBuilder AddWeavly(this IHostApplicationBuilder builder)
    {
        _weavlyApplicationBuilder = new WeavlyApplicationBuilder(builder);

        return _weavlyApplicationBuilder;
    }

    /// <summary>
    ///     Perform app start related tasks for all registered modules <see cref="IWeavlyModule" />
    /// </summary>
    /// <param name="app">
    ///     <see cref="WebApplication" />
    /// </param>
    /// <exception cref="InvalidOperationException">Will be thrown if no modules have been registered.</exception>
    public static void UseWeavly(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        foreach (var module in _weavlyApplicationBuilder?.Modules ?? [])
        {
            module.Use(app);
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
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
        var modules =
            _weavlyApplicationBuilder?.Modules ??
            throw new InvalidOperationException("Weavly has not been initialized");
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        app.MapOpenApi();
        if (app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference();
        }

        foreach (var module in modules)
        {
            module.Use(app);

            app.Lifetime.ApplicationStarted.Register(() => module.InitializeAsync(bus).Wait());
        }
    }
}
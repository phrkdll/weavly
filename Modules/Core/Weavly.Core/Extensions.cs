using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Weavly.Core.Implementation;
using Weavly.Core.Persistence;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core;

public static class Extensions
{
    private static WeavlyApplicationBuilder? _weavlyApplicationBuilder;

    /// <summary>
    ///     Add Weavly core components to the application
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns><see cref="IWeavlyApplicationBuilder"/></returns>
    public static IWeavlyApplicationBuilder AddWeavly(this WebApplicationBuilder builder)
    {
        _weavlyApplicationBuilder = new WeavlyApplicationBuilder(builder);

        return _weavlyApplicationBuilder;
    }

    /// <summary>
    ///     Perform app start related tasks for all registered modules <see cref="IWeavlyModule"/>
    /// </summary>
    /// <param name="app"><see cref="WebApplication"/></param>
    /// <exception cref="InvalidOperationException">Will be thrown if no modules have been registered.</exception>
    public static void UseWeavly(this WebApplication app)
    {
        var modules =
            _weavlyApplicationBuilder?.Modules
            ?? throw new InvalidOperationException("Weavly has not been initialized");
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        foreach (var module in modules)
        {
            module.Use(app);

            foreach (var dbContextType in module.DbContextTypes)
            {
                if (scope.ServiceProvider.GetRequiredService(dbContextType) is CoreDbContext ctx)
                {
                    ctx.Database.Migrate();
                }
            }

            app.Lifetime.ApplicationStarted.Register(() => module.InitializeAsync().Wait());
        }

        app.UseFastEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }

    /// <summary>
    ///     Find and register the correct provider-specific DbContext for a given module
    /// </summary>
    /// <typeparam name="TModule">The module containing the database context.</typeparam>
    /// <typeparam name="TDbContext">The base database context being registered.</typeparam>
    public static void AddWeavlyModuleDbContext<TModule, TDbContext>(this WebApplicationBuilder builder)
        where TDbContext : CoreDbContext
    {
        var contexts = typeof(TDbContext)
            .Assembly.GetExportedTypes()
            .Where(x => typeof(DbContext).IsAssignableFrom(x) && !x.IsAbstract)
            .ToDictionary(t => t.Name.Replace(typeof(TDbContext).Name, string.Empty).ToLower(), t => t);

        var moduleOptions = ContextOptions.RetrieveModuleOptions(builder.Configuration, typeof(TModule).Name);
        var provider =
            moduleOptions.DatabaseType
            ?? throw new ArgumentNullException($"No database type was specified for {typeof(TModule).Name}");

        var type = typeof(EntityFrameworkServiceCollectionExtensions);
        var method = type.GetMethods()
            .FirstOrDefault(i =>
                i is { Name: "AddDbContext", IsGenericMethod: true, IsStatic: true, IsPublic: true }
                && i.GetGenericArguments() is { Length: 2 }
            );

        var constructedMethod = method?.MakeGenericMethod(typeof(TDbContext), contexts[provider]);

        constructedMethod?.Invoke(null, [builder.Services, null, null, null]);
    }
}

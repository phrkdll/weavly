using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Persistence.Interceptors;

namespace Weavly.Core.Persistence;

public static class ContextOptions
{
    public static DbContextOptions CreateContextOptions(IServiceProvider serviceProvider, string moduleName)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var options = RetrieveModuleOptions(configuration, moduleName);

        var builder = new DbContextOptionsBuilder();
        switch (options.DatabaseType?.ToLower())
        {
            case "sqlite":
                PrepareSqlite(options, moduleName, builder);
                break;
            case "postgres":
                PreparePostgres(options, builder);
                break;
            default:
                throw new ArgumentException("Unsupported database type");
        }

        builder.UseStronglyTypeConverters();

        var interceptor = serviceProvider.GetRequiredService<MetaEntityInterceptor<AppUserId>>();
        builder.AddInterceptors(interceptor);

        return builder.Options;
    }

    public static ModuleOptions RetrieveModuleOptions(IConfiguration configuration, string moduleName)
    {
        var options = new ModuleOptions();

        var provider = configuration.GetValue("provider", null as string);

        if (provider != null)
        {
            options.DatabaseType = provider;
        }
        else
        {
            configuration.Bind(moduleName, options);
        }

        options.DatabaseType = options.DatabaseType?.ToLower();

        return options;
    }

    private static void PreparePostgres(ModuleOptions options, DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(
            options.ConnectionString ?? "Server=localhost;Port=5432;Database=Weavly;User Id=postgres;Password=postgres;"
        );
    }

    private static void PrepareSqlite(ModuleOptions options, string moduleName, DbContextOptionsBuilder builder)
    {
        var connectionString = options.ConnectionString ?? $"Data Source=data/{moduleName}.db";

        if (!Directory.Exists("data"))
        {
            Directory.CreateDirectory("data");
        }

        builder.UseSqlite(connectionString);
    }
}

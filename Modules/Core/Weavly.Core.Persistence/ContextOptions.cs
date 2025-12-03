using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Weavly.Core.Persistence;

public static class ContextOptions
{
    public static DbContextOptions CreateContextOptions(IServiceProvider serviceProvider, string moduleName)
    {
        var scopedProvider = serviceProvider.CreateScope().ServiceProvider;
        var configuration = scopedProvider.GetRequiredService<IConfiguration>();
        var fileSystem = scopedProvider.GetRequiredService<IFileSystem>();

        var options = RetrieveModuleOptions(configuration, moduleName);

        var builder = new DbContextOptionsBuilder();
        switch (options.DatabaseType?.ToLower())
        {
            case "sqlite":
                PrepareSqlite(options, moduleName, builder, fileSystem.Directory);
                break;
            case "postgres":
                PreparePostgres(options, builder);
                break;
            case "inmemory":
                PrepareInMemory(builder);
                break;
            default:
                throw new ArgumentException("Unsupported database type");
        }

        builder.UseStronglyTypeConverters();
        builder.AddInterceptors(scopedProvider.GetServices<ISaveChangesInterceptor>());

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

        options.DatabaseType = options.DatabaseType?.ToLower() ?? "inmemory";

        return options;
    }

    private static void PreparePostgres(ModuleOptions options, DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(options.ConnectionString);
    }

    private static void PrepareSqlite(
        ModuleOptions options,
        string moduleName,
        DbContextOptionsBuilder builder,
        IDirectory directory
    )
    {
        var connectionString = options.ConnectionString ?? $"Data Source=data/{moduleName}.db";

        if (!directory.Exists("data"))
        {
            directory.CreateDirectory("data");
        }

        builder.UseSqlite(connectionString);
    }

    private static void PrepareInMemory(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite("Data Source=:memory:");
    }
}

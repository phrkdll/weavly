﻿using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Weavly.Core.Persistence;

public static class ContextOptions
{
    public static DbContextOptions CreateContextOptions(IServiceProvider serviceProvider, string moduleName)
    {
        var scopedProvider = serviceProvider.CreateScope().ServiceProvider;
        var configuration = scopedProvider.GetRequiredService<IConfiguration>();

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

        options.DatabaseType = options.DatabaseType?.ToLower();

        return options;
    }

    private static void PreparePostgres(ModuleOptions options, DbContextOptionsBuilder builder)
    {
        builder.UseNpgsql(options.ConnectionString);
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

namespace Weavly.Configuration.Persistence;

public sealed class SqliteConfigurationDbContext(IServiceProvider serviceProvider)
    : ConfigurationDbContext(serviceProvider);

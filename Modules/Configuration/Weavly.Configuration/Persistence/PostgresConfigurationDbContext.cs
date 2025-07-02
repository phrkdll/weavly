namespace Weavly.Configuration.Persistence;

public sealed class PostgresConfigurationDbContext(IServiceProvider serviceProvider)
    : ConfigurationDbContext(serviceProvider);

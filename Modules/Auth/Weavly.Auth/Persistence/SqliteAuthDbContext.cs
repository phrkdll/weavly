namespace Weavly.Auth.Persistence;

public sealed class SqliteAuthDbContext(IServiceProvider serviceProvider) : AuthDbContext(serviceProvider);

namespace Weavly.Auth.Persistence;

public sealed class PostgresAuthDbContext(IServiceProvider serviceProvider) : AuthDbContext(serviceProvider);

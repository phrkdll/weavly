namespace Weavly.Core.Persistence;

public abstract class CoreDbContext : DbContext
{
    protected CoreDbContext() { }

    protected CoreDbContext(IServiceProvider serviceProvider, string moduleName)
        : base(ContextOptions.CreateContextOptions(serviceProvider, moduleName)) { }
}

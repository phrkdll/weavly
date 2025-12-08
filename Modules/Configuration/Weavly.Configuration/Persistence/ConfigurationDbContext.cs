using Microsoft.EntityFrameworkCore;
using Weavly.Configuration.Models;
using Weavly.Core.Persistence;

namespace Weavly.Configuration.Persistence;

public abstract class ConfigurationDbContext : CoreDbContext
{
    protected ConfigurationDbContext(IServiceProvider serviceProvider)
        : base(serviceProvider, nameof(ConfigurationModule)) { }

    protected ConfigurationDbContext(DbContextOptions options)
        : base(options) { }

    public virtual DbSet<AppConfiguration> Configurations { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigurationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

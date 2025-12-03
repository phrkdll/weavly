using Microsoft.EntityFrameworkCore;
using Weavly.Configuration.Models;
using Weavly.Core.Persistence;

namespace Weavly.Configuration.Persistence;

public abstract class ConfigurationDbContext : CoreDbContext
{
    public virtual DbSet<AppConfiguration> Configurations { get; init; }

    protected ConfigurationDbContext(IServiceProvider serviceProvider)
        : base(serviceProvider, nameof(ConfigurationModule)) { }

    protected ConfigurationDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigurationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

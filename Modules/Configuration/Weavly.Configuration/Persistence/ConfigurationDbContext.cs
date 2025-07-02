using Microsoft.EntityFrameworkCore;
using Weavly.Configuration.Models;
using Weavly.Core.Persistence;

namespace Weavly.Configuration.Persistence;

public abstract class ConfigurationDbContext(IServiceProvider serviceProvider)
    : CoreDbContext(serviceProvider, nameof(ConfigurationModule))
{
    public DbSet<AppConfiguration> Configurations { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigurationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

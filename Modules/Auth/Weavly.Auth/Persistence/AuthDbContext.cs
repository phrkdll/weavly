using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Models;
using Weavly.Core.Persistence;

namespace Weavly.Auth.Persistence;

public abstract class AuthDbContext(IServiceProvider serviceProvider)
    : CoreDbContext(serviceProvider, nameof(AuthModule))
{
    public DbSet<AppUser> Users { get; init; }

    public DbSet<AppRole> Roles { get; init; }

    public DbSet<AppUserToken> Tokens { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

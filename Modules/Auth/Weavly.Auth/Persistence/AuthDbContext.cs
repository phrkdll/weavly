using Microsoft.EntityFrameworkCore;
using Weavly.Auth.Models;
using Weavly.Core.Persistence;

namespace Weavly.Auth.Persistence;

public abstract class AuthDbContext : CoreDbContext
{
    public AuthDbContext(IServiceProvider serviceProvider)
        : base(serviceProvider, nameof(AuthModule)) { }

    protected AuthDbContext(DbContextOptions options)
        : base(options) { }

    public virtual DbSet<AppUser> Users { get; init; }

    public virtual DbSet<AppRole> Roles { get; init; }

    public virtual DbSet<AppUserToken> Tokens { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

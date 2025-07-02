using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Persistence.Configuration;

namespace Weavly.Auth.Models;

public class AppRoleEntitySetup : BaseEntitySetup<AppRole, AppRoleId>
{
    protected override void Setup(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("Role");

        builder.HasMany(x => x.Users).WithMany(x => x.Roles).UsingEntity(j => j.ToTable("UserRole"));
    }
}

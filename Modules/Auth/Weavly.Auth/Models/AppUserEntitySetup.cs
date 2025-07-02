using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Persistence.Configuration;

namespace Weavly.Auth.Models;

public class AppUserEntitySetup : BaseEntitySetup<AppUser, AppUserId>
{
    protected override void Setup(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("User");

        builder.Ignore(e => e.IsEmailVerified);
    }
}

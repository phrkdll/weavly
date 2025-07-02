using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Persistence.Configuration;

namespace Weavly.Auth.Models;

public class AppUserTokenEntitySetup : BaseEntitySetup<AppUserToken, AppUserTokenId>
{
    protected override void Setup(EntityTypeBuilder<AppUserToken> builder)
    {
        builder.ToTable("Token");
    }
}

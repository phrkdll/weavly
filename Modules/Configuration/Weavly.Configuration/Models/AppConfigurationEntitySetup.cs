using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Persistence.Configuration;

namespace Weavly.Configuration.Models;

public class AppConfigurationEntitySetup : BaseEntitySetup<AppConfiguration, ConfigurationId>
{
    protected override void Setup(EntityTypeBuilder<AppConfiguration> builder)
    {
        builder.ToTable("Configuration");
    }
}

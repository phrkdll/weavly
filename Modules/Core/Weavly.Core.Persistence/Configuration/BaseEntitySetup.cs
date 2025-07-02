using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Weavly.Core.Persistence.Configuration;

public abstract class BaseEntitySetup<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
    where TId : struct
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey("Id");
        builder.Property("Id").HasMaxLength(26).HasValueGenerator<StronglyGuidGenerator<TId>>().ValueGeneratedOnAdd();

        Setup(builder);
    }

    protected abstract void Setup(EntityTypeBuilder<TEntity> builder);
}

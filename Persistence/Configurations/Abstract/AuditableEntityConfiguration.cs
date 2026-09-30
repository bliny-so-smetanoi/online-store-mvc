using Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Abstract;

public abstract class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableEntity{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder) {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Created);
        builder.Property(x => x.LastModified);
        builder.Property(x => x.ModifyUser);
    }
}
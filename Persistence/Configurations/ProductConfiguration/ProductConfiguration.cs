using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configurations.Abstract;

namespace Persistence.Configurations.ProductConfiguration;

public class ProductConfiguration : AuditableEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasMany(x => x.Images).WithOne(x => x.Product).OnDelete(DeleteBehavior.Cascade);
    }
}
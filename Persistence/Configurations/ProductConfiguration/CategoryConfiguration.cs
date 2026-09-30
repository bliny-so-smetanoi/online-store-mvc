using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configurations.Abstract;

namespace Persistence.Configurations.ProductConfiguration;

public class CategoryConfiguration : AuditableEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        
        builder.ToTable("Categories");
        builder.HasOne(x => x.Parent).WithMany(x => x.Children).OnDelete(DeleteBehavior.NoAction);
    }
}
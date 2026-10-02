using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configurations.Abstract;

namespace Persistence.Configurations.OrderConfiguration;

public class CartConfiguration : AuditableEntityConfiguration<Cart>
{
    public override void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasMany(x => x.Products)
            .WithOne(x => x.Cart)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
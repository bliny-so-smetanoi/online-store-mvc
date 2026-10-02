using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configurations.Abstract;

namespace Persistence.Configurations.OrderConfiguration;

public class ProductInCartConfiguration : AuditableEntityConfiguration<ProductInCart>
{
    public override void Configure(EntityTypeBuilder<ProductInCart> builder)
    {
        builder.ToTable("ProductInCarts");
    }
}
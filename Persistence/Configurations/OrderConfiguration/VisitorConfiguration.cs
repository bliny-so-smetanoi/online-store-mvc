using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configurations.Abstract;

namespace Persistence.Configurations.OrderConfiguration;

public class VisitorConfiguration : AuditableEntityConfiguration<Visitor>
{
    public override void Configure(EntityTypeBuilder<Visitor> builder)
    {
        builder.ToTable("Visitors");
        builder.HasMany(x => x.Carts)
            .WithOne(x => x.Visitor)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
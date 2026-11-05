using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Configurations.Abstract;

namespace Persistence.Configurations.OrderConfiguration;

public class OrderConfiguration : AuditableEntityConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(x => x.Country).HasColumnName("AddressCountry").HasMaxLength(100).IsRequired();
            address.Property(x => x.City).HasColumnName("AddressCity").HasMaxLength(100).IsRequired();
            address.Property(x => x.Street).HasColumnName("AddressStreet").HasMaxLength(250).IsRequired();
            address.Property(x => x.PostalCode).HasColumnName("AddressPostalCode").HasMaxLength(20).IsRequired();
        });
        builder.Navigation(x => x.Address).IsRequired();

        builder.OwnsOne(x => x.PaymentInfo, payment =>
        {
            payment.Property(x => x.MaskedCardNumber).HasColumnName("PaymentMaskedCardNumber").HasMaxLength(19).IsRequired();
            payment.Property(x => x.ExpirationDate).HasColumnName("PaymentExpirationDate").HasMaxLength(5).IsRequired();
            payment.Property(x => x.CardHolderName).HasColumnName("PaymentCardHolderName").HasMaxLength(100).IsRequired();
        });
        builder.Navigation(x => x.PaymentInfo).IsRequired();
    }
}

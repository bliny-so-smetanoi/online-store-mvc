using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Orders;

public class Order : AuditableEntity
{
    public Cart Cart { get; protected set; }
    public Address Address { get; protected set; }
    public PaymentInfo PaymentInfo { get; protected set; }
    public OrderStatus Status { get; protected set; }
    
    protected Order() {}

    protected Order(Cart cart,
        Address address,
        PaymentInfo paymentInfo)
    {
        Cart = cart;
        Address = address;
        PaymentInfo = paymentInfo;
        Status = OrderStatus.Created;
    }

    public static Order Create(Cart cart, Address address, PaymentInfo paymentInfo)
    {
        return new Order(cart, address, paymentInfo);
    }
}
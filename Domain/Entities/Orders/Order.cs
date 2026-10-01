using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Orders;

public class Order : AuditableEntity
{
    public Cart Cart { get; protected set; }
    public OrderStatus Status { get; protected set; }
    
    protected  Order(){}

    protected Order(Cart cart)
    {
        Cart = cart;
        Status = OrderStatus.Created;
    }

    public static Order Create(Cart cart)
    {
        return new Order(cart);
    }
}
using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Orders;

public class Visitor : AuditableEntity
{
    public string Phone { get; protected set; }
    public string Email { get; protected set; }

    public List<Cart> Carts { get; protected set; } = new();
    
    protected  Visitor()
    {}

    protected Visitor(string phone, string email)
    {
        Phone = phone;
        Email = email;
    }

    public static Visitor Create(string phone, string email)
    {
        return new Visitor(phone, email);
    }

    public Cart? GetActiveCart()
    {
        return Carts.SingleOrDefault(x => x.Status == CartStatus.Created);
    }
}
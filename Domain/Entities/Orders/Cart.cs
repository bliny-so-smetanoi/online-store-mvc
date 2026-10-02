using Domain.Entities.Products;
using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Orders;

public class Cart : AuditableEntity
{
    public Visitor Visitor { get; protected set; }
    public List<ProductInCart> Products { get; protected set; } = new();
    public CartStatus Status { get; protected set; }
    
    
    
}
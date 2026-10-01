using Domain.Entities.Products;
using Domain.SeedWork;

namespace Domain.Entities.Orders;

public class ProductInCart : AuditableEntity
{
    public Product Product { get; protected set; }
    public Cart Cart { get; protected set; }
    public int Quantity { get; protected set; }
    
    protected  ProductInCart(){}

    protected ProductInCart(Product product, Cart cart, int quantity)
    {
        Product = product;
        Cart = cart;
        Quantity = quantity;
    }

    public static ProductInCart Create(Product product, Cart cart, int quantity)
    {
        return new ProductInCart(product, cart, quantity);
    }
}
using Domain.Entities.Products;
using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Orders;

public class Cart : AuditableEntity
{
    public Visitor Visitor { get; protected set; }
    public List<ProductInCart> Products { get; protected set; } = new();
    public CartStatus Status { get; protected set; }

    protected Cart(){}

    protected Cart(Visitor visitor)
    {
        Visitor = visitor;
        Status = CartStatus.Created;
    }

    public static Cart Create(Visitor visitor)
    {
        return new Cart(visitor);
    }

    public void AddProduct(Product product)
    {
        var productInCart = Products.FirstOrDefault(x => x.Product.Id == product.Id);
        if (productInCart is null)
        {
            Products.Add(ProductInCart.Create(product, this, 1));
        }
        else
        {
            productInCart.IncreaseQuantity(1);
        }
        MakeModified();
    }

    public int GetProductQuantity(Guid productId)
    {
        return Products.Where(x => x.Product.Id == productId).Sum(x => x.Quantity);
    }

    public void RemoveProduct(Product product)
    {
        var productForRemove = Products.FirstOrDefault(x => x.Product.Id == product.Id);
        if (productForRemove is null)
        {
            throw new Exception($"Product with id {product.Id} not found");
        }
        Products.Remove(productForRemove);
        MakeModified();
    }
}
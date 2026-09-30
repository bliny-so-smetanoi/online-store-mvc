using Domain.SeedWork;

namespace Domain.Entities.Products;

public class Product : AuditableEntity
{
    public string Name { get; protected set; }
    public decimal Price { get; protected set; }
    public string Description { get; protected set; }
    public int Quantity { get; protected set; }
    public Category? Category { get; protected set; }
    public IList<Image> Images { get; protected set; } = new List<Image>();
    protected Product()
    {
        
    }
    
    protected Product(string name, decimal price, string description, List<Image> images, int quantity, Category? category)
    {
        Name = name;
        Price = price;
        Description = description;
        Images = images;
        Quantity = quantity;
        Category = category;
    }

    public static Product Create(string name, decimal price, string description, List<Image> images,  int quantity, Category? category)
    {
        return new Product(name, price, description, images, quantity,  category);
    }

    public void Update(string name, decimal price, string description, int quantity, List<Image> images)
    {
        Name = name;
        Price = price;
        Description = description;
        if (images.Any())
        {
            Images = images;
        }
        Quantity = quantity;
    }
}
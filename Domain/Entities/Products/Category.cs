using Domain.SeedWork;

namespace Domain.Entities.Products;

public class Category : AuditableEntity
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Category? Parent { get; protected set; }
    
    public List<Category> Children { get; protected set; } = new List<Category>();

    public void Update(string name, string description, Category? parent)
    {
        Name = name;
        Description = description;
        Parent = parent;
    }

    public static Category Create(string name, string description, Category? parent)
    {
        return new Category
        {
            Name =  name,
            Description =  description,
            Parent = parent
        };
    }
    
    
}
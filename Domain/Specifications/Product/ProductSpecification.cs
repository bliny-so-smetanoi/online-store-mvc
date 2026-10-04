using NSpecifications;

namespace Domain.Specifications.Product;

public static class ProductSpecification
{
    public static ASpec<Entities.Products.Product> All() => Spec<Entities.Products.Product>.Any;
    public static ASpec<Entities.Products.Product> ById(Guid id) => new Spec<Entities.Products.Product>(x => x.Id == id);
    public static ASpec<Entities.Products.Product> ByName(string name) => new Spec<Entities.Products.Product>(x => x.Name.ToLower() == name.ToLower());
    
}
namespace Application.UseCases.Products.Dtos;

public class ProductDto
{
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
}


public class ProductWithImageDto : ProductDto
{
    public Guid? ImageId { get; set; }
}
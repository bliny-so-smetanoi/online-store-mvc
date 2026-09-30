

namespace Application.UseCases.Products.Dtos;

public class AddProductDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public Guid? CategoryId { get; set; }
    public List<ImageDto> Images { get; set; } = new List<ImageDto>();
}
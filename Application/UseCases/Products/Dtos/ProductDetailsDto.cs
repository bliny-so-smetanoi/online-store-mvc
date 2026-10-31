namespace Application.UseCases.Products.Dtos;

public class ProductDetailsDto : ProductDto
{
    public List<Guid> ImageIds { get; set; } = new List<Guid>();
}

namespace Application.UseCases.Products.Dtos;

public class UpdateViewProductDto : ProductDto
{
    public List<ImageDto> Images { get; set; } = new List<ImageDto>();
}
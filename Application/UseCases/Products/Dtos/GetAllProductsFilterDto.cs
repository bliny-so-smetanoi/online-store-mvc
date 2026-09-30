using Application.Dtos;

namespace Application.UseCases.Products.Dtos;

public class GetAllProductsFilterDto : PagedResultFilter
{
    public Guid? Id { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? Created { get; set; }
}
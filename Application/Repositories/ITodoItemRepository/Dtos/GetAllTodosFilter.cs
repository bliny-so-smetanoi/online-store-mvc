using Application.Dtos;

namespace Application.Repositories.ITodoItemRepository.Dtos;

public class GetAllTodosFilter : PagedResultFilter
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public DateTime? Created { get; set; }
}
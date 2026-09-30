namespace Application.UseCases.TodoItem.Dtos;

public class TodoItemDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime Created { get; set; }
    
}
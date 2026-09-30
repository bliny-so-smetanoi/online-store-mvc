using Application.UseCases.TodoItem.Dtos;

namespace Application.Services.ITodoItemService;

public interface ITodoItemService
{
    Task InsertAsync(TodoItemDto dto, CancellationToken cancellationToken);
    Task<TodoItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
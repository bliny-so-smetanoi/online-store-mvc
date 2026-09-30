using Application.Dtos;
using Application.Repositories.ITodoItemRepository.Dtos;
using Domain.Entities.TodoItem;
using NSpecifications;

namespace Application.Repositories.ITodoItemRepository;

public interface ITodoItemRepository
{
    Task InsertAsync(TodoItem item, CancellationToken cancellationToken);
    Task<TodoItem> GetOneAsync(Spec<TodoItem> spec, CancellationToken cancellationToken);
    Task<PagedResult<TodoItem>> GetAllPagedAsync(GetAllTodosFilter? options, CancellationToken cancellationToken);
}
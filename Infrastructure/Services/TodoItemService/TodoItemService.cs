using Application.Repositories;
using Application.Repositories.ITodoItemRepository;
using Application.Services;
using Application.Services.ITodoItemService;
using Application.UseCases.TodoItem.Dtos;
using Domain.Entities;
using Domain.Entities.TodoItem;
using Domain.Specifications;
using Domain.Specifications.TodoItem;

namespace Infrastructure.Services.TodoItemService;

public class TodoItemService : ITodoItemService
{
    private readonly ITodoItemRepository _repository;

    public TodoItemService(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task InsertAsync(TodoItemDto dto, CancellationToken cancellationToken)
    {
        var todoItem = new TodoItem()
        {
            Title = dto.Title,
        };

        await _repository.InsertAsync(todoItem, cancellationToken);
    }

    public async Task<TodoItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _repository.GetOneAsync(TodoItemSpecification.GetById(id), cancellationToken);
        if (item == null)
        {
            throw new KeyNotFoundException();
        }

        return new TodoItemDto() { Id = item.Id, Title = item.Title};
    }
}
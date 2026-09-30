using Application.Abstractions;
using Application.Repositories.ITodoItemRepository;
using Application.UseCases.TodoItem.Dtos;
using CSharpFunctionalExtensions;
using Domain.Specifications.TodoItem;

namespace Application.UseCases.TodoItem.Queries.GetById;

public class GetTodoTaskByIdQueryHandler : IQueryHandler<GetTodoTaskByIdQuery, Result<TodoItemDto>>
{
    private readonly ITodoItemRepository _repository;

    public GetTodoTaskByIdQueryHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<TodoItemDto>> Handle(GetTodoTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetOneAsync(TodoItemSpecification.GetById(request.Id), cancellationToken);
        var result = new TodoItemDto()
        {
            Id = item.Id,
            Title = item.Title,
        };
        return Result.Success(result);
    }
}
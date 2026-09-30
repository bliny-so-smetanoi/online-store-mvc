using Application.Abstractions;
using Application.Dtos;
using Application.Repositories.ITodoItemRepository.Dtos;
using Application.UseCases.TodoItem.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.TodoItem.Queries.GetAll;

public class GetAllTodosQuery : IQuery<Result<PagedResult<TodoItemDto>>>
{
    public GetAllTodosFilter? Filter { get; set; }

    public GetAllTodosQuery(GetAllTodosFilter filter)
    {
        Filter = filter;
    }
}
using Application.Abstractions;
using Application.UseCases.TodoItem.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.TodoItem.Queries.GetById;

public class GetTodoTaskByIdQuery : IQuery<Result<TodoItemDto>>
{
    public Guid Id { get; set; }

    public GetTodoTaskByIdQuery(Guid id)
    {
        Id = id;
    }
}
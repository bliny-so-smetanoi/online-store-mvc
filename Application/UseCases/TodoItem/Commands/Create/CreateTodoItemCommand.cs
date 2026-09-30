using Application.UseCases.TodoItem.Dtos;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.UseCases.TodoItem.Commands.Create;

public class CreateTodoItemCommand : IRequest<Result>
{
    public TodoItemDto Data { get; set; }

    public CreateTodoItemCommand(TodoItemDto data)
    {
        Data = data;
    }
}
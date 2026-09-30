using Application.Mediator;
using FluentValidation;

namespace Application.UseCases.TodoItem.Commands.Create;

public class CreateTodoItemCommandValidator : RequestValidatorBase<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(x => x.Data.Title).NotEmpty();
    }
    
}
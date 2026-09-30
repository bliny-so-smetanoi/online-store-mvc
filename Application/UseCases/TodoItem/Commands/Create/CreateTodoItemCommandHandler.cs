using Application.Services.ITodoItemService;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.UseCases.TodoItem.Commands.Create;

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Result>
{
    private readonly ITodoItemService _service;

    public CreateTodoItemCommandHandler(ITodoItemService service)
    {
        _service = service;
    }

    public async Task<Result> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        await _service.InsertAsync(request.Data, cancellationToken);
        return Result.Success();
    }
}
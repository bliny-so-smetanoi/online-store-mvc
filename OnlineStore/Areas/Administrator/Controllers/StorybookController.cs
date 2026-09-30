using Application.Repositories.ITodoItemRepository.Dtos;
using Application.UseCases.TodoItem.Commands.Create;
using Application.UseCases.TodoItem.Dtos;
using Application.UseCases.TodoItem.Queries.GetAll;
using KeycloakExtension;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Helpers;

namespace OnlineStore.Areas.Administrator.Controllers;

[Authorize(Roles = RoleConstants.Root)]
[Area("Administrator")]
public class StorybookController(IMediator mediator) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddTodo([FromBody] TodoItemDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateTodoItemCommand(dto);
        var result = await mediator.Send(command, cancellationToken);
        return result.AsHttpResult();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetAllTodosFilter filter, CancellationToken cancellationToken)
    {
        var query = new GetAllTodosQuery(filter);
        var result = await mediator.Send(query, cancellationToken);
        return result.AsHttpResult();
    }
}
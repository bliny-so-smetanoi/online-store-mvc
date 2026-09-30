using Application.Abstractions;
using Application.Dtos;
using Application.Repositories.ITodoItemRepository;
using Application.UseCases.TodoItem.Dtos;
using CSharpFunctionalExtensions;

namespace Application.UseCases.TodoItem.Queries.GetAll;

public class GetAllTodosQueryHandler : IQueryHandler<GetAllTodosQuery, Result<PagedResult<TodoItemDto>>>
{
    private readonly ITodoItemRepository _repository;

    public GetAllTodosQueryHandler(ITodoItemRepository repository)
    {
        _repository = repository;   
    }
    
    public async Task<Result<PagedResult<TodoItemDto>>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetAllPagedAsync(request.Filter, cancellationToken);
        var listDto = list.Items.Select(x => new TodoItemDto()
        {
            Id = x.Id,
            Title = x.Title,
            Created = x.Created,
        }).ToList();
        
        var totalCount = list.TotalCount;
        var page = list.Page;
        var pageSize = list.PageSize;
        var result = new PagedResult<TodoItemDto>()
        {
            Items = listDto,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
        return Result.Success(result);
    }
}
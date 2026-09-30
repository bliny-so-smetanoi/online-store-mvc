using Application.Dtos;
using Application.Repositories.ITodoItemRepository;
using Application.Repositories.ITodoItemRepository.Dtos;
using Domain.Entities.TodoItem;
using Microsoft.EntityFrameworkCore;
using NSpecifications;
using Persistence.Context;

namespace Persistence.Repositories.TodoItemRepository;

public class TodoItemRepository : ITodoItemRepository
{
    private readonly AppDbContext _context;

    public TodoItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task InsertAsync(TodoItem item, CancellationToken cancellationToken)
    {
        await _context.AddAsync(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TodoItem> GetOneAsync(Spec<TodoItem> spec, CancellationToken cancellationToken)
    {
        return await _context.Set<TodoItem>().FirstOrDefaultAsync(spec);
    }

    public async Task<PagedResult<TodoItem>> GetAllPagedAsync(GetAllTodosFilter? options, CancellationToken cancellationToken)
    {
        if (options.Page <= 0) options.Page = 1;
        if (options.PageSize <= 0) options.PageSize = 10;

        var query = _context.Set<TodoItem>().AsQueryable();

        if (options is not null)
        {
            if (!string.IsNullOrWhiteSpace(options.Title))
            {
                query = query.Where(t => t.Title.Contains(options.Title));
            }

            if (options.Id is not null)
            {
                query = query.Where(t => t.Id == options.Id.Value);
            }

            if (options.Created is not null)
            {
                var date = options.Created.Value.Date;
                query = query.Where(t => t.Created.Date == date);
            }
        }

        query = query.OrderByDescending(x => x.Created);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TodoItem>
        {
            Items = items,
            TotalCount = totalCount,
            Page = options.Page,
            PageSize = options.PageSize
        };
    }

}
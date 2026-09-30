using System.Linq.Expressions;
using Application.Dtos;
using Application.Repositories.IProductRepository;
using Application.UseCases.Products.Dtos;
using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using NSpecifications;
using Persistence.Context;

namespace Persistence.Repositories.ProductRepository;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<bool> AnyAsync(ASpec<Product> spec, CancellationToken cancellationToken = default)
    {
        return await context.Products.AnyAsync(spec, cancellationToken);
    }
    
    public async Task<Product?> GetAsync(ASpec<Product> spec, CancellationToken cancellationToken = default, params Expression<Func<Product, object>>[] includes)
    {
        IQueryable<Product> query = context.Products;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.SingleOrDefaultAsync(spec, cancellationToken);
    }

    public async Task<PagedResult<ProductDto>> GetAllPagedAsync(GetAllProductsFilterDto? options, CancellationToken cancellationToken = default)
    {
        if (options.Page <= 0) options.Page = 1;
        if (options.PageSize <= 0) options.PageSize = 10;

        var query = context.Set<Product>().AsQueryable();

        if (options is not null)
        {
            if (!string.IsNullOrWhiteSpace(options.Name))
            {
                query = query.Where(t => t.Name.Contains(options.Name));
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
            .Select(x => new ProductDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Quantity = x.Quantity,
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = options.Page,
            PageSize = options.PageSize
        };
    }


    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
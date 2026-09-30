using Application.Repositories.ICategoryRepository;
using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories.CategoryRepository;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task CreateAsync(Category category, CancellationToken cancellationToken)
    {
        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Category>> GetAllCategories(CancellationToken cancellationToken)
    {
        return context.Categories.AsNoTracking().Include(x => x.Parent)
            .OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Categories.Include(x => x.Parent)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        category.MakeModified();
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Products.AnyAsync(x => x.CategoryId == id, cancellationToken)
            || await context.Categories.AnyAsync(x => x.Parent != null && x.Parent.Id == id, cancellationToken);
    }

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken)
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
    }
}
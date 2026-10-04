using Application.Repositories.Orders;
using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories.Orders;

public class VisitorRepository(AppDbContext context) : IVisitorRepository
{
    public async Task CreateAsync(Visitor visitor, CancellationToken cancellationToken)
    {
        await context.Visitors.AddAsync(visitor, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Visitor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Visitors.Include(x => x.Carts)
            .ThenInclude(x => x.Products)
            .SingleOrDefaultAsync(x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<Visitor>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Visitors.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Visitor visitor, CancellationToken cancellationToken)
    {
        context.Visitors.Update(visitor);
        await context.SaveChangesAsync(cancellationToken);
    }
}
